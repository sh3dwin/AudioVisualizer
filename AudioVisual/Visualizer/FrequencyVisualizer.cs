using System.Collections.Generic;
using System.Windows.Controls;
using AudioVisual.DataStructures;
using AudioVisual.Processor;

namespace AudioVisual.Visualizer
{
    public class FrequencyVisualizer : IProcessedFrequencySpectrumVisualizer
    {
        private readonly FrequencyBinAggregator _processor;
        private readonly FrequencySpectrumVisualizer _visualizer;

        public FrequencyVisualizer(FrequencyBinAggregator processor, FrequencySpectrumVisualizer visualizer)
        {
            _processor = processor;
            _visualizer = visualizer;
        }
        public Canvas Draw(List<FftFrequencyBin> frequencySpectrum)
        {
            var aggregatedFrequencies = _processor.GetAggregatedFrequencies(frequencySpectrum);
            return _visualizer.Draw(aggregatedFrequencies);
        }

        public void Dispose()
        {
            _visualizer?.Dispose();
        }
    }
}
