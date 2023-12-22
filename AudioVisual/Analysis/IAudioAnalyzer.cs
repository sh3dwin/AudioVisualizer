using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioVisual.DataStructures;
using NAudio.Dsp;
using NAudio.Wave;

namespace AudioVisual.Analysis
{
    public interface IAudioAnalyzer
    {
        public List<FftFrequencyBin> GetFrequencySpectrum(WaveBuffer audioData, int m);
    }
}
