using System.Collections.Generic;
using AudioVisual.DataStructures;
using NAudio.Wave;

namespace AudioVisual.Analyzers
{
    public interface IAudioAnalyzer
    {
        List<FftFrequencyBin> Analyze(WaveBuffer audioData, int m);
        List<double> Synthesize(FrequencyFilter filter, int m);
    }
}
