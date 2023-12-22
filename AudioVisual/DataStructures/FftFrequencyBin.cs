using System;
using NAudio.Dsp;

namespace AudioVisual.DataStructures
{
    public class FftFrequencyBin
    {
        public float Amplitude { get; set; }
        public float Frequency { get; set; }
        public float PhaseShift { get; set; }

        public FftFrequencyBin(Complex fftBin, float frequency)
        {
            Amplitude = Math.Abs(fftBin.X);
            PhaseShift = fftBin.Y;
            Frequency = frequency;
        }

        public Complex ToComplex()
        {
            return new Complex()
            {
                X = (float)Amplitude,
                Y = (float)PhaseShift
            };
        }
    }
}
