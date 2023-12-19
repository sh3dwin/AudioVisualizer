using System.Windows.Controls;
using AudioVisual.Processor;
using NAudio.Dsp;

namespace AudioVisual.Visualizer
{
    public class FrequencyVisualizer : IProcessedFrequencySpectrumVisualizer
    {
        private readonly FrequencySpectrumAggregator _processor;
        private readonly FrequencySpectrumVisualizer _visualizer;

        public FrequencyVisualizer(FrequencySpectrumAggregator processor, FrequencySpectrumVisualizer visualizer)
        {
            _processor = processor;
            _visualizer = visualizer;
        }
        public Canvas Draw(Complex[] frequencySpectrum)
        {
            var aggregatedFrequencies = _processor.GetAggregatedFrequencies(frequencySpectrum);
            var hues = _processor.GetHues();
            return _visualizer.Draw(aggregatedFrequencies, hues);
        }
    }
}
