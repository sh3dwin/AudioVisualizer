using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using AudioVisual.Audio;
using AudioVisual.Processor;
using AudioVisual.Visualizer;

namespace AudioVisual.ViewModel
{
    public class PlaybackViewModel : ViewModelBase
    {
        private readonly ISongProvider _songProvider;
        private SoundSource _selectedSong;
        private readonly AudioStreamPlayer _player;
        private readonly DispatcherTimer _timer;
        // recorder
        private readonly IAudioProcessor _recorder;
        // visualizers
        private IProcessedFrequencySpectrumVisualizer _visualizer;
        // analyzer
        private readonly FourierTransformAnalyzer _analyzer;
        private float _elapsedTime;
        private bool _isPlaying;
        private readonly int _segmentCount = 200;
        private int _wavePartitions = 1;
        private Canvas _canvas;
        private bool _waveVisualization;

        // The size of the FFT, 2^M
        private int M = 12;
        public PlaybackViewModel(Canvas canvas)
        {
            _songProvider = new AudioStreamProvider();
            _player = new AudioStreamPlayer();
            _canvas = canvas;
            _recorder = new LoopbackAudioProcessor();
            WaveVisualization = false;
            if (_waveVisualization)
            {
                _visualizer = new WaveVisualizer(
                    new SubBandFilterBank(_segmentCount, _wavePartitions),
                    new SubBandFilterBankVisualizer(_canvas));
            }
            else
            {
                _visualizer = new FrequencyVisualizer(
                    new FrequencySpectrumAggregator(_segmentCount),
                    new FrequencySpectrumVisualizer(canvas));
            }
            _analyzer = new FourierTransformAnalyzer();

            Play = new PlaybackCommand(PlaySong, CanPlay);
            Stop = new PlaybackCommand(StopSong, CanStop);

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(33)
            };
            _timer.Tick += Timer_Tick;

            // Start the timer
            _timer.Start();
        }

        public PlaybackCommand Play { get; }
        public PlaybackCommand Stop { get; }

        public ObservableCollection<SoundSource> Songs
        {
            get;
        } = new();

        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                _isPlaying = value;
                Play.RaiseCanExecuteChanges();
                Stop.RaiseCanExecuteChanges();
                RaisePropertyChanged(nameof(IsPlaying));
            }
        }

        public Canvas Visualization
        {
            get => _canvas;
            set
            {
                _canvas = value;
                RaisePropertyChanged(nameof(Visualization));
            }
        }
        public SoundSource SelectedSong
        {
            get => _selectedSong;
            set
            {
                _selectedSong = value;
                _player.Open(_selectedSong.FileName);
                Play.RaiseCanExecuteChanges();
                Stop.RaiseCanExecuteChanges();
                RaisePropertyChanged(nameof(SelectedSong));
            }
        }

        public float ElapsedTimePercentage
        {
            get => _elapsedTime;
            set
            {
                _elapsedTime = value;
                RaisePropertyChanged(nameof(ElapsedTimePercentage));
            }
        }

        public bool WaveVisualization
        {
            get => _waveVisualization;
            set
            {
                _waveVisualization = value;
                FrequencyVisualization = !_waveVisualization;

                if (_waveVisualization)
                {
                    _visualizer = new WaveVisualizer(
                        new SubBandFilterBank(_segmentCount, _wavePartitions),
                        new SubBandFilterBankVisualizer(_canvas));
                }
                else
                {
                    _visualizer = new FrequencyVisualizer(
                        new FrequencySpectrumAggregator(_segmentCount),
                        new FrequencySpectrumVisualizer(_canvas));
                }
                RaisePropertyChanged(nameof(FrequencyVisualization));
                RaisePropertyChanged(nameof(WaveVisualization));
            }
        }

        public bool FrequencyVisualization { get; set; }

        public int WavePartitions
        {
            get => _wavePartitions;
            set
            {
                _wavePartitions = value;
                if (_visualizer.GetType() == typeof(WaveVisualizer))
                    (_visualizer as WaveVisualizer)?.SetBandPassCount(value);
                RaisePropertyChanged(nameof(WavePartitions));
            }
        }

        public async override Task LoadAsync()
        {
            if (Songs.Any())
            {
                return;
            }

            var songs = _songProvider.GetAllSongs();
            if (Songs is not null)
            {
                foreach (var song in songs.Library)
                {
                    Songs.Add(song);
                }
            }
        }

        private void PlaySong(object? parameter)
        {
            if (SelectedSong is null) return;
            IsPlaying = true;
            _player.Play();
        }
        private void StopSong(object? parameter)
        {
            if (SelectedSong is null) return;
            IsPlaying = false;
            _player.Stop();
        }

        private bool CanPlay(object? parameter)
        {
            return !IsPlaying && SelectedSong != null;
        }

        private bool CanStop(object? parameter)
        {
            return IsPlaying && SelectedSong != null;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            ElapsedTimePercentage = (float)_player.Position.TotalMilliseconds / (float)_player.Length.TotalMilliseconds;
            var audioData = _recorder.GetAudioData(); 
            var frequencySpectrum = _analyzer.GetFrequencySpectrum(audioData, M);
            Visualization = _visualizer.Draw(frequencySpectrum);
        }

        public override void Dispose()
        {
            _player?.Dispose();
            _recorder.Dispose();
        }
    }
}
