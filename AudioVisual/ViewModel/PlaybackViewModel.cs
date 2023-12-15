﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using AudioVisual.Visualizer;
using AudioVisual.Audio;
using System.Diagnostics;
using AudioVisual.Processor;

namespace AudioVisual
{
    public class PlaybackViewModel : ViewModelBase, IDisposable
    {
        private readonly ISongProvider _songProvider;
        private SoundSource _selectedSong;
        private readonly AudioStreamPlayer _player;
        private readonly IAudioProcessor _recorder;
        private readonly DispatcherTimer _timer;
        private readonly FreqVisualizerNAudio _visualizer;
        private readonly FrequencySpectrumAggregator _processor;
        private readonly FourierTransformAnalyzer _analyzer;
        private float _elapsedTime = 0;
        private bool _isPlaying;
        private Canvas _canvas;
        public PlaybackViewModel(ISongProvider songProvider, AudioStreamPlayer player, Canvas canvas)
        {
            _player = player;
            _songProvider = songProvider;
            this.LoadAsync();
            //_visualizer = new FreqVisualizerNAudio(canvas);
            _recorder = new LoopbackAudioProcessor();
            _visualizer = new FreqVisualizerNAudio(canvas);
            _processor = new FrequencySpectrumAggregator(100);
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
            var frequencySpectrum = _analyzer.GetFrequencySpectrum(audioData, 12);
            var summedWavesValues = _processor.GetAggregatedFrequencies(frequencySpectrum);
            var hues = _processor.GetHues();
            Visualization = _visualizer.Draw(summedWavesValues, hues);
        }

        public override void Dispose()
        {
            _player?.Dispose();
            _recorder.Dispose();
        }
    }
}
