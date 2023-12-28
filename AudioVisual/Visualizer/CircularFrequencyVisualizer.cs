using AudioVisual.DataStructures;
using AudioVisual.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

namespace AudioVisual.Visualizer
{
    internal class CircularFrequencyVisualizer : VisualizerAbstract
    {
        private const double RadiusScalingFactor = 0.5;
        protected override List<List<Line>> DrawLines(List<List<double>> visualizationData)

        {
            var normalizedVisualizationData =
                visualizationData
                    .NormalizeValues()
                    .ToList();

            var mirroredFrequencySpectrum = visualizationData
                .NormalizeValues()
                .ToList()[0];
            mirroredFrequencySpectrum.Reverse();

            normalizedVisualizationData[0].AddRange(mirroredFrequencySpectrum);

            var count = visualizationData.Count;

            var visualizableList = new List<List<Line>>(count);

            for (var i = 0; i < count; i++)
            {
                // Line information
                var lines = ToColorGradientCircle(normalizedVisualizationData[i]);

                visualizableList.Add(lines);
            }

            return visualizableList;
        }

        protected override List<DrawingLayoutData> GetLayouts(int layoutCount, double width, double height)
        {
            if (layoutCount != 1)
                throw new Exception("Cannot draw more than one frequency spectrum!");

            var drawingLayouts = new List<DrawingLayoutData>(layoutCount);

            var circleCenterX = width / 2.0;
            var circleCenterY = height / 2.0;

            var radius = Math.Min(width * 0.5, height * 0.5) * RadiusScalingFactor;

            var center = new Point(circleCenterX, circleCenterY);

            var layout = new DrawingLayoutData(center, radius, radius, null);
            drawingLayouts.Add(layout);

            return drawingLayouts;
        }

        private static List<Line> ToColorGradientCircle(IReadOnlyList<double> lineData)
        {
            var lines = new List<Line>();

            var maxFluctuation = lineData.Max() * 0.5;

            var fluctuationFirst = 1 + maxFluctuation * lineData[0];
            var pointFirst = new Point(fluctuationFirst, -Math.PI * 0.5).PolarToCartesian();

            var x1 = pointFirst.X;
            var y1 = pointFirst.Y;

            var angleStep = (Math.PI * 2) / (lineData.Count);

            var hues = FrequencyToColorMapper.GetListOfHues(lineData.Count);

            for (var iSegment = 1; iSegment <= lineData.Count; iSegment++)
            {
                var theta = angleStep * iSegment - Math.PI * 0.5;

                var hue = hues[iSegment - 1];
                var color = FrequencyToColorMapper.ColorFromHsv(hue, 1, 1);
                var brush = new SolidColorBrush(color);

                if (iSegment != (lineData.Count))
                {

                    var fluctuation = 1 + maxFluctuation * lineData[iSegment];
                    var point = new Point(fluctuation, theta).PolarToCartesian();

                    var line = new Line
                    {
                        X1 = x1,
                        Y1 = y1,
                        X2 = point.X,
                        Y2 = point.Y,
                        Stroke = brush
                    };

                    lines.Add(line);

                    x1 = line.X2;
                    y1 = line.Y2;
                }
                else
                {
                    var fluctuation = 1 + maxFluctuation * lineData[0];
                    var point = new Point(fluctuation, theta).PolarToCartesian();
                    var line = new Line
                    {
                        X1 = x1,
                        Y1 = y1,
                        X2 = point.X,
                        Y2 = point.Y,
                        Stroke = brush
                    };

                    lines.Add(line);
                    break;
                }
            }

            return lines;
        }
    }
}
