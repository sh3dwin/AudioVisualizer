using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using AudioVisual.Analysis;
using AudioVisual.Audio;
using AudioVisual.DataStructures;
using AudioVisual.Processor;
using AudioVisual.Utils;
using AudioVisual.Visualizer;
using NAudio.Dsp;

namespace AudioVisual.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        // recorder
        private readonly IAudioProcessor _recorder;
        // visualizers
        private IProcessedFrequencySpectrumVisualizer _visualizer;
        // analyzer
        private readonly FourierTransformAnalyzer _analyzer;
        private int _wavePartitions = Constants.DefaultWavePartitions;
        private Canvas _canvas;
        private bool _wavePartitionsBarVisibilityVisibility;
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
                UpdateVisualizationMode();
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
            get => _wavePartitionsBarVisibilityVisibility;
            set
            {
                _wavePartitionsBarVisibilityVisibility = value;
                RaisePropertyChanged();
            }
        }

        public int WavePartitions
        {
            get => _wavePartitions;
            set
            {
                _wavePartitions = value;
                if (VisualizationMode == Enums.VisualizationMode.Wave)
                    (_visualizer as WaveVisualizer)?.SetBandPassCount(value);
                else if (VisualizationMode == Enums.VisualizationMode.Circular)
                    (_visualizer as CircularWaveVisualizer)?.SetBandPassCount(value);
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
            var frequencySpectrum = _analyzer.GetFrequencySpectrum(audioData, Constants.PowerOfTwo);
            //var doubleSpec = _analyzer.GetFrequencySpectrumAsDoubleArray(audioData, Constants.PowerOfTwo);

            //var reverseFreq = FourierTransformAnalyzer.FrequencySpectrumToTimeDomain(
            //    doubleSpec.ToArray(),
            //    Constants.PowerOfTwo);

            //var frequencySpectrumDoubles = _analyzer.GetFrequencySpectrumAsDoubleArray(audioData, Constants.PowerOfTwo);
            //var frequencySpectrumComplexArray = new List<Complex>(frequencySpectrumDoubles.Count);
            //frequencySpectrumComplexArray.AddRange(frequencySpectrumDoubles.Select(x => new Complex() { X = x.X, Y = 0}));
            //var backToWave = FourierTransformAnalyzer.FrequencySpectrumToTimeDomain(frequencySpectrumComplexArray.ToArray(), Constants.PowerOfTwo);

            //var wave = new FrequencyFilter(frequencySpectrum, 0, 24000);
            //var values = wave.ToPaddedComplexArray(Constants.PowerOfTwo).Select(x => (double)Math.Abs(x.X)).ToList();
            //var reversefft = FourierTransformAnalyzer.FrequencySpectrumToTimeDomain(wave.ToPaddedComplexArray(Constants.PowerOfTwo), Constants.PowerOfTwo);

            Visualization.Dispatcher.Invoke(() => 
                { Visualization = _visualizer.Draw(frequencySpectrum); });
        }

        private void UpdateVisualizationMode()
        {
            switch (_visualizationMode)
            {
                case Enums.VisualizationMode.Frequency:
                {
                    _visualizer = new FrequencyVisualizer(
                        new FrequencyBinAggregator(),
                        new FrequencySpectrumVisualizer(_canvas));
                    WavePartitionsBarVisibility = false;
                    break;
                }
                case Enums.VisualizationMode.Circular:
                {
                    _visualizer = new CircularWaveVisualizer(
                        new FilterBank(_wavePartitions),
                        new CircularFilterBankVisualizer(_canvas));
                    WavePartitionsBarVisibility = true;
                    break;
                }
                case Enums.VisualizationMode.Wave:
                {
                    _visualizer = new WaveVisualizer(
                        new FilterBank(_wavePartitions),
                        new FilterBankVisualizer(_canvas));
                    WavePartitionsBarVisibility = true;
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
