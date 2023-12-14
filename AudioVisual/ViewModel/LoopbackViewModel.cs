using AudioVisual.Audio;
using AudioVisual.Visualizer;
using SharpDX.Direct2D1;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace AudioVisual
{
    public class LoopbackViewModel: ViewModelBase, IDisposable
    {
    private readonly IAudioProcessor _processor;
    private readonly DispatcherTimer _timer;
    private readonly IVisualizer _visualizer;
    private float _elapsedTime = 0;
    private Canvas _canvas;
    public LoopbackViewModel(Canvas canvas)
    {
        this.LoadAsync();
        //_visualizer = new FreqVisualizerNAudio(canvas);
        _processor = new LoopbackAudioProcessor();
        _visualizer = new WaveVisualizer(canvas, ((LoopbackAudioProcessor)_processor).SampleRate);

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

    private void Timer_Tick(object sender, EventArgs e)
    {
        Visualization = _visualizer.Draw(new FourierTransformAnalyzer().GetFrequencySpectrum(_processor.GetAudioData(), 12));
    }

    public void Dispose()
    {
        _processor.Dispose();
    }
}
}
