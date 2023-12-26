using System.Collections.Generic;
using System.Windows.Controls;
using AudioVisual.DataStructures;
using AudioVisual.Processor;

namespace AudioVisual.Visualizer
{
    public class WaveVisualizer : IProcessedDataVisualizer
    {
        private readonly FilterBank _processor = new();
        private readonly FilterBankVisualizer _visualizer = new();

        public Canvas Draw(Canvas canvas, List<FftFrequencyBin> frequencySpectrum, int wavePartitionsCount)
        {
            var filterBank = _processor.GetFrequencyFilters(frequencySpectrum, wavePartitionsCount);
            return _visualizer.Draw(canvas,filterBank);
        }

        public void Dispose()
        {
            _visualizer?.Dispose();
        }
    }
}
