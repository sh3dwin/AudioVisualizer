using System;
using System.Collections.Generic;
using System.Windows.Controls;
using AudioVisual.DataStructures;

namespace AudioVisual.Visualizer
{
    public interface IProcessedFrequencySpectrumVisualizer : IDisposable
    {
        public Canvas Draw(List<FftFrequencyBin> frequencySpectrum);
    }
}
