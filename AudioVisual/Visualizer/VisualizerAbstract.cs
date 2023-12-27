using System.Collections.Generic;
using System.Windows.Shapes;
using AudioVisual.DataStructures;
using AudioVisual.Utils;

namespace AudioVisual.Visualizer
{
    public abstract class VisualizerAbstract : IVisualizer
    {
        protected abstract List<List<Line>> DrawLines(List<List<double>> visualizationData);

        protected abstract List<DrawingLayoutData> GetLayouts(int layoutCount, double width, double height);

        public List<Shape> GetDrawables(List<List<double>> visualizationData, int waveCount, double width, double height)
        {
            var lines = DrawLines(visualizationData);
            var drawingLayouts = GetLayouts(visualizationData.Count, width, height);

            var result = new List<Shape>();

            for (var iElement = 0; iElement < lines.Count; iElement++)
            {
                var drawable = lines[iElement].GetDrawable(drawingLayouts[iElement]);

                drawable.ForEach(x => result.Add(x));
            }

            return result;
        }
        
    }
}
