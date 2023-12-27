using System.Collections.Generic;
using System.Linq;
using AudioVisual.Analyzers;
using AudioVisual.DataStructures;
using AudioVisual.Processor;
using AudioVisual.Utils;

namespace AudioVisual.AudioVisualConverters
{
    public class WaveConverter : IFilterBankConverter
    {
        public FilterBank Processor { get; } = new ();

        public List<List<double>> Convert(IAudioAnalyzer analyzer, List<FftFrequencyBin> frequencySpectrum, int wavePartitionsCount)
        {
            var filterBank = Processor.GetFrequencyFilters(frequencySpectrum, wavePartitionsCount);

            var waves = filterBank.Select(filter => analyzer.Synthesize(filter, Constants.PowerOfTwo)).ToList();

            return waves;
        }
    }
}
