using System;
using System.Windows.Controls;
using AudioVisual.Analyzers;
using AudioVisual.DataStructures;
using AudioVisual.Recorders;
using AudioVisual.Utils;

namespace AudioVisual.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        // recorder
        private readonly IAudioRecorder _recorder;
        // Canvas
        private Canvas _canvas;
        // Options
        private readonly UiOptions _options;

        public MainViewModel(Canvas canvas)
        {
            // Recorder
            _recorder = new LoopbackAudioRecorder();
            _recorder.DataAvailableEvent += DrawBuffer;

            _options = new UiOptions();

            _canvas = canvas;
        }

        public Enums.VisualizationMode VisualizationMode
        {
            get => _options.VisualizationMode;
            set
            {
                _options.VisualizationMode = value;
                _options.Update();
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
            get => _options.WavePartitionsBarVisibility;
            set
            {
                _options.WavePartitionsBarVisibility = value;
                RaisePropertyChanged();
            }
        }

        public int WavePartitions
        {
            get => _options.WavePartitions;
            set
            {
                _options.WavePartitions = value;
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

            var analyzer = new FourierTransformAnalyzer();
            var frequencySpectrum = analyzer.Analyze(audioData, Constants.PowerOfTwo);

            var converter = Providers.ConverterProvider.GetConverter(VisualizationMode);
            var visualizationData = converter.Convert(analyzer, frequencySpectrum, WavePartitions);

            var visualizer = Providers.VisualizerProvider.GetVisualizer(converter);

            Visualization.Dispatcher.Invoke(() =>
            {
                var drawables = visualizer.GetDrawables(visualizationData, WavePartitions, Visualization.ActualWidth, Visualization.ActualHeight);

                Visualization.Clear();

                drawables.ForEach(drawable => Visualization.Children.Add(drawable));
            });
        }
    }
}
