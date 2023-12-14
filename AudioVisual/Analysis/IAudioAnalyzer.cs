using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Dsp;
using NAudio.Wave;

namespace AudioVisual.Analysis
{
    public interface IAudioAnalyzer
    {
        public Complex[] GetFrequencySpectrum(WaveBuffer audioData, int m);
    }
}
