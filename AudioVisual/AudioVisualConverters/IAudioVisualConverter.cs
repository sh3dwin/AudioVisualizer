using System.Collections.Generic;
using AudioVisual.Analyzers;
using AudioVisual.DataStructures;
using AudioVisual.Processor;

namespace AudioVisual.AudioVisualConverters
{
    public interface IAudioVisualConverter<out T>
        where T : IProcessor
    {
        T Processor { get; }
        public List<List<double>> Convert(IAudioAnalyzer analyzer, List<FftFrequencyBin> frequencySpectrum, int wavePartitionsCount);
    }
    public interface IFilterBankConverter: IAudioVisualConverter<FilterBank>
    {
    }
}
