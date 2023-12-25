using System.Collections.Generic;
using System.Windows.Controls;
using AudioVisual.DataStructures;
using AudioVisual.Processor;
using NAudio.Dsp;

namespace AudioVisual.Visualizer
{
    public class WaveVisualizer : IProcessedFrequencySpectrumVisualizer
    {
        private readonly FilterBank _processor;
        private readonly FilterBankVisualizer _visualizer;

        public WaveVisualizer(FilterBank processor, FilterBankVisualizer visualizer)
        {
            _processor = processor;
            _visualizer = visualizer;

        }

        public Canvas Draw(List<FftFrequencyBin> frequencySpectrum)
        {
            var filterBank = _processor.GetFilterBank(frequencySpectrum);
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
