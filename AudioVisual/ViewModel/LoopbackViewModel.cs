using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using AudioVisual.Audio;
using AudioVisual.Processor;
using AudioVisual.Utils;
using AudioVisual.Visualizer;

namespace AudioVisual.ViewModel
{
    public class LoopbackViewModel : ViewModelBase
    {
        private CircleVisualizer temp;

        // recorder
        private readonly IAudioProcessor _recorder;
        // visualizers
        private IProcessedFrequencySpectrumVisualizer _visualizer;
        // analyzer
        private readonly FourierTransformAnalyzer _analyzer;
        private readonly int _segmentCount = 200;
        private int _wavePartitions = 1;
        private Canvas _canvas;
        private bool _wavePartitionsBar = false;
        private bool _waveVisualization = false;
        private bool _circularWaveVisualization = false;
        private bool _frequencyVisualization = false;
        public LoopbackViewModel(Canvas canvas)
        {
            MathUtils.InitializeSinValuesBetween0AndPi();
            _canvas = canvas;

            temp = new CircleVisualizer(canvas);

            _recorder = new LoopbackAudioProcessor();
            WaveVisualization = false;
            CircularWaveVisualization = false;
            WavePartitionsBar = false;
            FrequencyVisualization = true;
            _visualizer = new FrequencyVisualizer(
                new FrequencySpectrumAggregator(_segmentCount),
                new FrequencySpectrumVisualizer(canvas));

            _analyzer = new FourierTransformAnalyzer();
            _recorder.DataAvailableEvent += DrawBuffer;
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
        public bool FrequencyVisualization
        {
            get => _frequencyVisualization;
            set
            {
                _frequencyVisualization = value;
                WavePartitionsBar = !_frequencyVisualization;
                _circularWaveVisualization = !_frequencyVisualization;
                _waveVisualization = !_frequencyVisualization;

                if (_frequencyVisualization)
                {
                    _visualizer = new FrequencyVisualizer(
                        new FrequencySpectrumAggregator(_segmentCount),
                        new FrequencySpectrumVisualizer(_canvas));
                }
                RaisePropertyChanged(nameof(FrequencyVisualization));
            }
        }

        public bool WaveVisualization
        {
            get => _waveVisualization;
            set
            {
                _waveVisualization = value;
                _frequencyVisualization = !_waveVisualization;
                _circularWaveVisualization = !_waveVisualization;

                if (_waveVisualization)
                {
                    _visualizer = new WaveVisualizer(
                        new SubBandFilterBank(_segmentCount, _wavePartitions),
                        new SubBandFilterBankVisualizer(_canvas));
                }
                RaisePropertyChanged(nameof(WaveVisualization));
            }
        }

        public bool CircularWaveVisualization
        {
            get => _circularWaveVisualization;
            set
            {
                _circularWaveVisualization = value;
                _frequencyVisualization = !_circularWaveVisualization;
                _waveVisualization = !_circularWaveVisualization;

                if (_circularWaveVisualization)
                {
                    _visualizer = new CircularWaveVisualizer(
                        new SubBandFilterBank(_segmentCount, _wavePartitions),
                        new CircularSubBandFilterBankVisualizer(_canvas));
                }
                RaisePropertyChanged(nameof(CircularWaveVisualization));
            }
        }

        public bool WavePartitionsBar
        {
            get => _wavePartitionsBar;
            set
            {
                _wavePartitionsBar = value;
                RaisePropertyChanged(nameof(WavePartitionsBar));
            }
        }

        public int WavePartitions
        {
            get => _wavePartitions;
            set
            {
                _wavePartitions = value;
                if (_visualizer.GetType() == typeof(WaveVisualizer))
                    (_visualizer as WaveVisualizer)?.SetBandPassCount(value);
                else if (_visualizer.GetType() == typeof(CircularWaveVisualizer))
                    (_visualizer as CircularWaveVisualizer)?.SetBandPassCount(value);
                RaisePropertyChanged(nameof(WavePartitions));
            }
        }

        private void DrawBuffer(object sender, EventArgs e)
        {
            var audioData = _recorder.GetAudioData();
            var frequencySpectrum = _analyzer.GetFrequencySpectrum(audioData, 12);

            Visualization.Dispatcher.Invoke(() => 
                { Visualization = temp.Draw(frequencySpectrum); });
        }

        public override void Dispose()
        {
            _recorder.Dispose();
        }
    }
}
