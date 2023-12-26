using System;
using System.Collections.Generic;
using System.Windows.Controls;
using AudioVisual.DataStructures;

namespace AudioVisual.Visualizer
{
    public interface IProcessedDataVisualizer : IDisposable
    {
        public Canvas Draw(Canvas canvas, List<FftFrequencyBin> frequencySpectrum);
    }
}
