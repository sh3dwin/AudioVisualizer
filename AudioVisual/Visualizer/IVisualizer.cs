﻿using System.Windows.Controls;
using NAudio.Dsp;

namespace AudioVisual.Visualizer
{
    public interface IVisualizer
    {
        public Canvas Draw(Complex[] frequencySpectrum);
    }
}
