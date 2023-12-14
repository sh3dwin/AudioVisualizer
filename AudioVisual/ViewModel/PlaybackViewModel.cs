using CSCore.Utils;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using AudioVisual.Visualizer;
using System.Diagnostics;

namespace AudioVisual
{
    public class PlaybackViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly ISongProvider _songProvider;
        private SoundSource _selectedSong;
        private AudioStreamPlayer _player;
        private DispatcherTimer _timer;
        private IVisualizer _visualizer;
        private float _elapsedTime = 0;
        private bool _isPlaying;
        private Complex[] buffer = new Complex[2048];
        private Canvas _canvas;
        public PlaybackViewModel(ISongProvider songProvider, AudioStreamPlayer player, Canvas canvas)
        {
            _player = player;
            _songProvider = songProvider;
            //_visualizer = new FreqVisualizerNAudio(canvas);
            _visualizer = new WaveVisualizer(canvas, 0);

            Play = new PlaybackCommand(PlaySong, CanPlay);
            Stop = new PlaybackCommand(StopSong, CanStop);

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(1); // Set the interval to 1 second
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

        public async Task LoadAsync()
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

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

            Visualization = _visualizer.Draw(new FourierTransformAnalyzer().GetFrequencySpectrum(null, 12));
        }

        public void Dispose()
        {
            _player?.Dispose();
        }
    }
}
