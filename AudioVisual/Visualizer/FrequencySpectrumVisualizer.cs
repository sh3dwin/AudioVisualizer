using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using AudioVisual.DataStructures;
using AudioVisual.Utils;

namespace AudioVisual.Visualizer
{
    public class FrequencySpectrumVisualizer : VisualizerAbstract
    {
        protected override List<List<Line>> DrawLines(List<List<double>> visualizationData)

        {
            // Take the negative values of the visualization data so the lines can be draw from the bottom of the canvas
            var normalizedVisualizationData = 
                visualizationData
                    .NormalizeValues()
                    .Select(x => x.Select(value => -value).ToList())
                    .ToList();

            var count = visualizationData.Count;

            var visualizableList = new List<List<Line>>(count);

            for (var i = 0; i < count; i++)
            {
                // Line information
                var lines = ToColorGradientLines(normalizedVisualizationData[i]);

                visualizableList.Add(lines);
            }

            return visualizableList;
        }

        protected override List<DrawingLayoutData> GetLayouts(int layoutCount, double width, double height)
        {
            var drawingLayouts = new List<DrawingLayoutData>(layoutCount);
            // Position
            var scaleX = width / Constants.SegmentCount;
            var scaleY = height;

            for (var iLayout = 0; iLayout < layoutCount; iLayout++)
            {
                var startX = 0.0;
                var startY = height;
                var center = new Point(startX, startY);

                // Color information
                var hue = FrequencyToColorMapper.GetListOfHues(layoutCount)[iLayout];
                var color = FrequencyToColorMapper.ColorFromHsv(hue, 1, 1);
                var brush = new SolidColorBrush(color);

                var layout = new DrawingLayoutData(center, scaleX, scaleY, brush);
                drawingLayouts.Add(layout);
            }

            return drawingLayouts;
        }

        private List<Line> ToColorGradientLines(IReadOnlyList<double> lineData)
        {
            var y1 = lineData[0];

            var lines = new List<Line>(Constants.SegmentCount);

            var hues = FrequencyToColorMapper.GetListOfHues(Constants.SegmentCount);


            for (var iSegment = 1; iSegment < Constants.SegmentCount; iSegment++)
            {
                var hue = hues[iSegment];
                var color = FrequencyToColorMapper.ColorFromHsv(hue, 1, 1);
                var brush = new SolidColorBrush(color);

                var line = new Line
                {
                    X1 = iSegment,
                    Y1 = y1,
                    X2 = (iSegment + 1),
                    Y2 = lineData[iSegment],
                    Stroke = brush
                };
                lines.Add(line);

                y1 = line.Y2;
            }

            return lines;
        }
    }
}