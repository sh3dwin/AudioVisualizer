using AudioVisual.Processor;
using System.Windows.Controls;
using NAudio.Dsp;
using AudioVisual.DataStructures;
using System.Collections.Generic;

namespace AudioVisual.Visualizer
{
    public class CircularWaveVisualizer : IProcessedFrequencySpectrumVisualizer
    {
        private readonly FilterBank _processor;
        private readonly CircularFilterBankVisualizer _visualizer;

        public CircularWaveVisualizer(FilterBank processor, CircularFilterBankVisualizer visualizer)
        {
            _processor = processor;
            _visualizer = visualizer;

        }

        public Canvas Draw(List<FftFrequencyBin> frequencySpectrum)
        {
            var filterBank = _processor.GetSubBandFilterBank(frequencySpectrum);
            return _visualizer.Draw(filterBank);
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
