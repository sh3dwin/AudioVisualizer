using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using NAudio.Dsp;

namespace AudioVisual.Visualizer
{
    public interface IVisualizer
    {
        public Canvas Draw(Complex[] frequencySpectrum);
    }
}
