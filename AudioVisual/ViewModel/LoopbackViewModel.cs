using AudioVisual.Audio;
using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace AudioVisual
{
    public class LoopbackViewModel: ViewModelBase, IDisposable
    {
    private readonly IAudioProcessor _recorder;
    private readonly DispatcherTimer _timer;
    private readonly WaveVisualizer _visualizer;
    private readonly FrequencyIntoWaveAggregator _processor;
    private readonly FourierTransformAnalyzer _analyzer;
    private float _elapsedTime = 0;
    private int _samplesNumber = 200;
    private int _wavePartitions = 1;
    private Canvas _canvas;
    public LoopbackViewModel(Canvas canvas)
    {
        this.LoadAsync();
        //_visualizer = new FreqVisualizerNAudio(canvas);
        _recorder = new LoopbackAudioProcessor();
        _visualizer = new WaveVisualizer(canvas);
        _processor = new FrequencyIntoWaveAggregator(_samplesNumber,_wavePartitions);
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

    public float ElapsedTimePercentage
    {
        get => _elapsedTime;
        set
        {
            _elapsedTime = value;
            RaisePropertyChanged(nameof(ElapsedTimePercentage));
        }
    }

    public int WavePartitions
    {
        get => _wavePartitions;
        set
        {
            _wavePartitions = value;
            _processor.WavePartitionNumber = _wavePartitions;
            RaisePropertyChanged(nameof(WavePartitions));
        }
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        var audioData = _recorder.GetAudioData();
        var frequencySpectrum = _analyzer.GetFrequencySpectrum(audioData, 12);
        var summedWavesValues = _processor.GetSummedWaves(frequencySpectrum);
        var hues = _processor.GetHues();
        Visualization = _visualizer.Draw(summedWavesValues, hues);
    }

    public override void Dispose()
    {
        _recorder.Dispose();
    }
}
}
