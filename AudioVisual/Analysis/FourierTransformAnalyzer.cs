using System;
using System.Collections.Generic;
using System.Linq;
using AudioVisual.DataStructures;
using AudioVisual.Utils;
using NAudio.Dsp;
using NAudio.Wave;

namespace AudioVisual.Analysis
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

        public List<Complex> GetFrequencySpectrumAsDoubleArray(WaveBuffer buffer, int m)
        {
            var audioData = SoundWaveUtils.CreateAndInitializeComplexArray(buffer, m);
            FastFourierTransform.FFT(true, m, audioData);

            // return the transformed data
            return audioData.ToList();
        }

        public static List<FftFrequencyBin> ConvertToFftFrequencyBins(Complex[] fftData)
        {
            var count = fftData.Length;

            var fftFrequencyBins = new List<FftFrequencyBin>(count);

            var binWidthInFrequency = (float)(Constants.SampleRate) / (count);

            // Add frequencies in ascending order in the first half of the discrete fourier
            for (var i = 0; i < count; i++)
            {
                var frequency = (float)binWidthInFrequency * i;
                if (i > count / 2)
                {
                    frequency = (float)(Constants.SampleRate - frequency);
                }
                
                fftFrequencyBins.Add(new FftFrequencyBin(fftData[i], frequency, i));
            }

            return fftFrequencyBins;
        }

        public static List<double> FrequencySpectrumToTimeDomain(Complex[] frequencySpectrum, int m)
        {
            if (frequencySpectrum.Length != (int)Math.Pow(2, m))
            {
                throw new Exception("Size mismatch in FFT array size. Must be power of 2!");
            }

            var timeDomainBuffer = (Complex[])frequencySpectrum.Clone();

            FastFourierTransform.FFT(false, m, timeDomainBuffer);

            return timeDomainBuffer.Select(x => (double)x.X).ToList().GetRange(0, timeDomainBuffer.Length / 2);
        }

        public static List<double> ToWave(FrequencyFilter filter, int m)
        {
            var wholeFrequencySpectrum = filter.ToPaddedComplexArray(m);

            var waveAmplitudes = FrequencySpectrumToTimeDomain(wholeFrequencySpectrum, m);

            return waveAmplitudes;
        }
    }
}
