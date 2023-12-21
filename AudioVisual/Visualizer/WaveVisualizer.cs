using System.Windows.Controls;
using AudioVisual.Processor;
using NAudio.Dsp;

namespace AudioVisual.Visualizer
{
    public class WaveVisualizer : IProcessedFrequencySpectrumVisualizer
    {
        private readonly SubBandFilterBank _processor;
        private readonly SubBandFilterBankVisualizer _visualizer;

        public WaveVisualizer(SubBandFilterBank processor, SubBandFilterBankVisualizer visualizer)
        {
            _processor = processor;
            _visualizer = visualizer;

        }

        public Canvas Draw(Complex[] frequencySpectrum)
        {
            var filterBank = _processor.GetSubBandFilterBank(frequencySpectrum);
            return _visualizer.Draw(filterBank);
        }

        public void SetBandPassCount(int bandPassCount)
        {
            _processor.BandPassCount = bandPassCount;
        }
    }
}
