using NAudio.Dsp;

namespace AudioVisual.DataStructures
{
    public class FftFrequencyBin
    {
        public float Amplitude { get; set; }
        public float Frequency { get; set; }
        public float PhaseShift { get; set; }

        public int BinIndex { get; set; }

        public FftFrequencyBin(Complex fftBin, float frequency, int binIndex)
        {
            Amplitude = (float)(fftBin.X);
            PhaseShift = fftBin.Y;
            Frequency = frequency;
            BinIndex = binIndex;
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
