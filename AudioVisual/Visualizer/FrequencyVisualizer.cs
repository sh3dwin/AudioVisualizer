using System.Collections.Generic;
using System.Windows.Controls;
using AudioVisual.DataStructures;
using AudioVisual.Processor;

namespace AudioVisual.Visualizer
{
    public class FrequencyVisualizer : IProcessedDataVisualizer
    {
        private readonly FilterBank _processor = new();
        private readonly FrequencySpectrumVisualizer _visualizer = new();

        public Canvas Draw(Canvas canvas, List<FftFrequencyBin> frequencySpectrum, int _)
        {
            var aggregatedFrequencies = _processor.GetAggregatedFrequencies(frequencySpectrum);
            return _visualizer.Draw(canvas, aggregatedFrequencies);
        }

        public void Dispose()
        {
            _visualizer?.Dispose();
        }
    }
}
