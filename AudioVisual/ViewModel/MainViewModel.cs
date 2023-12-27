using System;
using System.Windows.Controls;
using AudioVisual.Analyzers;
using AudioVisual.AudioVisualConverters;
using AudioVisual.Recorders;
using AudioVisual.Utils;

namespace AudioVisual.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        // recorder
        private readonly IAudioRecorder _recorder;
        // visualizers
        private IFilterBankConverter _converter;
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
            _recorder = new LoopbackAudioRecorder();
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

            var visualizationData = _converter.Convert(_analyzer, frequencySpectrum, _wavePartitions);

            var visualizer = Providers.VisualizerProvider.GetVisualizer(_converter);

            

            Visualization.Dispatcher.Invoke(() =>
            {
                var drawables = visualizer.GetDrawables(visualizationData, WavePartitions, Visualization.ActualWidth, Visualization.ActualHeight);

                Visualization.Clear();

                drawables.ForEach(drawable => Visualization.Children.Add(drawable));
            });
        }

        private void UpdateUi()
        {
            _converter = Providers.ConverterProvider.GetConverter(_visualizationMode);

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
