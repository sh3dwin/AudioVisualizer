using AudioVisual.DataStructures;
using System.Collections.Generic;

namespace AudioVisual.Processor
{
    public interface IProcessor
    {
        List<double> GetAggregatedFrequencies(List<FftFrequencyBin> fftResult);
        List<FrequencyFilter> GetFrequencyFilters(List<FftFrequencyBin> fftResult, int wavePartitionsCount);
    }
}
