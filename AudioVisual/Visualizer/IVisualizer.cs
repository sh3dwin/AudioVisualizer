using System.Collections.Generic;
using System.Windows.Shapes;

namespace AudioVisual.Visualizer
{
    public interface IVisualizer
    {
        public List<Shape> GetDrawables(List<List<double>> visualizationData,
            int waveCount, double width, double height);
    }
}
