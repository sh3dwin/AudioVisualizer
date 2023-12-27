using System.Collections.Generic;
using AudioVisual.Analyzers;
using AudioVisual.DataStructures;
using AudioVisual.Processor;

namespace AudioVisual.AudioVisualConverters
{
    public class FrequencyConverter : IFilterBankConverter
    {
        public FilterBank Processor { get; } = new FilterBank();

        public List<List<double>> Convert(IAudioAnalyzer _, List<FftFrequencyBin> frequencySpectrum, int __)
        {
            return new List<List<double>> {Processor.GetAggregatedFrequencies(frequencySpectrum)};
        }

    }
}
