using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AudioVisual.Visualizer
{
    public interface IVisualizer
    {
        public Canvas Draw();
        public void Dispose();
    }
}
