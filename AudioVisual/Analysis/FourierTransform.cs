using System;
using AudioVisual.Analysis;
using NAudio.Dsp;
using NAudio.Wave;

namespace AudioVisual
{
    public class FourierTransformAnalyzer : IAudioAnalyzer
    {
        public Complex[] GetFrequencySpectrum(WaveBuffer buffer, int m)
        {
            var audioData = SoundWaveUtils.CreateAndInitializeComplexArray(buffer, m);
            FastFourierTransform.FFT(true, m, audioData);

            // Take only the first 1/4 elements, rest is duplicate
            var result = new Complex[(int)(audioData.Length * 0.25)];
            Array.Copy(audioData, result, result.Length);
            return result;
        }
    }
}
