using System;
using System.Windows.Controls;
using AudioVisual.Analysis;
using AudioVisual.Audio;
using AudioVisual.Utils;
using AudioVisual.Visualizer;

namespace AudioVisual.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        // recorder
        private readonly IAudioProcessor _recorder;
        // visualizers
        private IProcessedDataVisualizer _visualizer;
        // analyzer
        private readonly FourierTransformAnalyzer _analyzer;

        // Canvas
        private Canvas _canvas;

        // Options
        private int _wavePartitions = Constants.DefaultWavePartitions;
        private bool _wavePartitionsBarVisibility;
        private Enums.VisualizationMode _visualizationMode = Constants.DefaultVisualizationMode;

        public MainViewModel(Canvas canvas)
        {
            // Recorder
            _recorder = new LoopbackAudioProcessor();
            _recorder.DataAvailableEvent += DrawBuffer;

            // Analyzer
            _analyzer = new FourierTransformAnalyzer();

            //Visualizer
            _canvas = canvas;
            VisualizationMode = Constants.DefaultVisualizationMode;
        }

        public Enums.VisualizationMode VisualizationMode
        {
            get => _visualizationMode;
            set
            {
                _visualizationMode = value;
                UpdateUi();
                RaisePropertyChanged();
            }
        }

        public Canvas Visualization
        {
            get => _canvas;
            set
            {
                _canvas = value;
                RaisePropertyChanged();
            }
        }

        public bool WavePartitionsBarVisibility
        {
            get => _wavePartitionsBarVisibility;
            set
            {
                _wavePartitionsBarVisibility = value;
                RaisePropertyChanged();
            }
        }

        public int WavePartitions
        {
            get => _wavePartitions;
            set
            {
                _wavePartitions = value;
                RaisePropertyChanged();
            }
        }
        public override void Dispose()
        {
            _recorder.Dispose();
        }

        private void DrawBuffer(object sender, EventArgs e)
        {
            var audioData = _recorder.GetAudioData();
            var frequencySpectrum = _analyzer.Analyze(audioData, Constants.PowerOfTwo);

            Visualization.Dispatcher.Invoke(() => 
                { Visualization = _visualizer.Draw(_canvas, frequencySpectrum, _wavePartitions); });
        }

        private void UpdateUi()
        {
            _visualizer = Providers.VisualizerProvider.GetProcessedDataVisualizer(_visualizationMode);

            switch (_visualizationMode)
            {
                case Enums.VisualizationMode.Frequency:
                {
                    WavePartitionsBarVisibility = false;
                    break;
                }
                case Enums.VisualizationMode.Circular:
                {
                    WavePartitionsBarVisibility = true;
                    break;
                }
                case Enums.VisualizationMode.Wave:
                {
                    WavePartitionsBarVisibility = true;
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
