using System.Collections.Generic;
using System.Windows.Controls;
using AudioVisual.DataStructures;
using NAudio.Dsp;

namespace AudioVisual.Visualizer
{
    public interface IVisualizer
    {
        public Canvas Draw(Canvas canvas, List<FftFrequencyBin> frequencySpectrum);
    }
}
