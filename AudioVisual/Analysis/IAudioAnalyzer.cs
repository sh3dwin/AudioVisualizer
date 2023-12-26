using System.Collections.Generic;
using AudioVisual.DataStructures;
using NAudio.Wave;

namespace AudioVisual.Analysis
{
    public interface IAudioAnalyzer
    {
        public List<FftFrequencyBin> Analyze(WaveBuffer audioData, int m);
        public List<double> Synthesize(FrequencyFilter filter, int m);
    }
}
