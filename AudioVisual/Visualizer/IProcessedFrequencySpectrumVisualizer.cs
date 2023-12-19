using System.Windows.Controls;
using NAudio.Dsp;

namespace AudioVisual.Visualizer
{
    public interface IProcessedFrequencySpectrumVisualizer
    {
        public Canvas Draw(Complex[] frequencySpectrum);
    }
}
