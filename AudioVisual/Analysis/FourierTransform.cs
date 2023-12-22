using System;
using System.Collections.Generic;
using System.Linq;
using AudioVisual.Analysis;
using AudioVisual.DataStructures;
using AudioVisual.Utils;
using NAudio.Dsp;
using NAudio.Wave;

namespace AudioVisual
{
    public class FourierTransformAnalyzer : IAudioAnalyzer
    {
        public List<FftFrequencyBin> GetFrequencySpectrum(WaveBuffer buffer, int m)
        {
            var audioData = SoundWaveUtils.CreateAndInitializeComplexArray(buffer, m);
            FastFourierTransform.FFT(true, m, audioData);

            // return the transformed data
            return ConvertToFftFrequencyBins(audioData);
        }

        public static List<FftFrequencyBin> ConvertToFftFrequencyBins(Complex[] fftData)
        {
            // get only the first 1/4, or the bins containing data of the first 12kHz
            var count = fftData.Length;

            var fftFrequencyBins = new List<FftFrequencyBin>(count);

            var maxSignalFrequency = 24000;
            var binWidthInFrequency = (float)maxSignalFrequency / count;

            for (var i = 0; i < count; i++)
            {
                var frequency = binWidthInFrequency * i;

                fftFrequencyBins.Add(new FftFrequencyBin(fftData[i], frequency));
            }

            return fftFrequencyBins;
        }

        public static List<float> FrequencySpectrumToTimeDomain(Complex[] frequencySpectrum, int m)
        {
            if (frequencySpectrum.Length != (int)Math.Pow(2, m))
            {
                throw new Exception("Size mismatch in FFT array size. Must be power of 2!");
            }

            var timeDomainBuffer = (Complex[])frequencySpectrum.Clone();

            FastFourierTransform.FFT(false, m, timeDomainBuffer);

            return timeDomainBuffer.Select(x => x.X).ToList();
        }

        public static List<float> ToWave(FrequencyFilter filter, int m)
        {
            var wholeFrequencySpectrum = filter.ToPaddedComplexArray(m);

            var waveAmplitudes = FrequencySpectrumToTimeDomain(wholeFrequencySpectrum, m);

            return waveAmplitudes;
        }
    }
}
