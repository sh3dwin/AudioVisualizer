using AudioVisual.Processor;
using System.Windows.Controls;
using AudioVisual.DataStructures;
using System.Collections.Generic;

namespace AudioVisual.Visualizer
{
    public class CircularWaveVisualizer : IProcessedDataVisualizer
    {
        private readonly FilterBank _processor;
        private readonly CircularFilterBankVisualizer _visualizer;

        public CircularWaveVisualizer(FilterBank processor, CircularFilterBankVisualizer visualizer)
        {
            _processor = processor;
            _visualizer = visualizer;

        }

        public Canvas Draw(Canvas canvas, List<FftFrequencyBin> frequencySpectrum)
        {
            var filterBank = _processor.GetFilterBank(frequencySpectrum);
            return _visualizer.Draw(canvas, filterBank);
        }

        public void SetBandPassCount(int bandPassCount)
        {
            _processor.BandPassCount = bandPassCount;
        }

        public void Dispose()
        {
            _visualizer?.Dispose();
        }
    }
}
