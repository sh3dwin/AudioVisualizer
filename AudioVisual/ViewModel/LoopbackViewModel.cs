using System;
using System.Windows.Controls;
using System.Windows.Threading;
using AudioVisual.Audio;
using AudioVisual.Processor;
using AudioVisual.Visualizer;

namespace AudioVisual.ViewModel
{
    public class LoopbackViewModel: ViewModelBase
    {
    private readonly DispatcherTimer _timer;
    // recorder
    private readonly IAudioProcessor _recorder;
    // visualizers
    private IProcessedFrequencySpectrumVisualizer _visualizer;
    // analyzer
    private readonly FourierTransformAnalyzer _analyzer;
    private readonly int _segmentCount = 200;
    private int _wavePartitions = 1;
    private Canvas _canvas;
    private bool _waveVisualization = false;
    public LoopbackViewModel(Canvas canvas)
    {
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

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(33)
        };
        _timer.Tick += Timer_Tick;

        // Start the timer
        _timer.Start();
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
            if(_visualizer.GetType() == typeof(WaveVisualizer))
                (_visualizer as WaveVisualizer)?.SetBandPassCount(value);
            RaisePropertyChanged(nameof(WavePartitions));
        }
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        var audioData = _recorder.GetAudioData();
        var frequencySpectrum = _analyzer.GetFrequencySpectrum(audioData, 12);
        Visualization = _visualizer.Draw(frequencySpectrum);
    }

    public override void Dispose()
    {
        _recorder.Dispose();
    }
}
}
