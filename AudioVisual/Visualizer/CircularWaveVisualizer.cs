using AudioVisual.Processor;
using System.Windows.Controls;
using AudioVisual.DataStructures;
using System.Collections.Generic;

namespace AudioVisual.Visualizer
{
    public class CircularWaveVisualizer : IProcessedDataVisualizer
    {
        private readonly FilterBank _processor = new();
        private readonly CircularFilterBankVisualizer _visualizer = new();

        public Canvas Draw(Canvas canvas, List<FftFrequencyBin> frequencySpectrum, int wavePartitionsCount)
        {
            var filterBank = _processor.GetFrequencyFilters(frequencySpectrum, wavePartitionsCount);
            return _visualizer.Draw(canvas, filterBank);
        }

        public void Dispose()
        {
            _visualizer?.Dispose();
        }
    }
}
