using System;
using System.Collections.Generic;
using System.Linq;
using AudioVisual.DataStructures;
using AudioVisual.Utils;
using NAudio.Dsp;
using NAudio.Wave;

namespace AudioVisual.Analyzers
{
    public class FourierTransformAnalyzer : IAudioAnalyzer
    {
        public List<FftFrequencyBin> Analyze(WaveBuffer buffer, int m)
        {
            var audioData = GeneralUtils.CreateAndInitializeComplexArray(buffer);
            FastFourierTransform.FFT(true, m, audioData);

            // return the transformed data
            return ToBins(audioData);
        }

        private static List<FftFrequencyBin> ToBins(Complex[] fftData)
        {
            var count = fftData.Length;

            var fftFrequencyBins = new List<FftFrequencyBin>(count);

            var binWidthInFrequency = (float)(Globals.SampleRate) / (count);

            // Add frequencies in ascending order in the first half of the discrete fourier
            for (var i = 0; i < count; i++)
            {
                var frequency = binWidthInFrequency * i;
                if (i > count / 2)
                {
                    frequency = (Globals.SampleRate - frequency);
                }
                
                fftFrequencyBins.Add(new FftFrequencyBin(fftData[i], frequency, i));
            }

            return fftFrequencyBins;
        }

        public List<double> Synthesize(FrequencyFilter filter, int m)
        {
            var wholeFrequencySpectrum = filter.ToPaddedComplexArray(m);

            var waveAmplitudes = ReverseFft(wholeFrequencySpectrum, m);

            return waveAmplitudes;
        }

        private List<double> ReverseFft(Complex[] frequencySpectrum, int m)
        {
            if (frequencySpectrum.Length != (int)Math.Pow(2, m))
            {
                throw new Exception("Size mismatch in FFT array size. Must be power of 2!");
            }

            var timeDomainBuffer = (Complex[])frequencySpectrum.Clone();

            FastFourierTransform.FFT(false, m, timeDomainBuffer);

            return timeDomainBuffer.Select(x => (double)x.X).ToList().GetRange(0, timeDomainBuffer.Length / 2);
        }
    }
}
