using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using AudioVisual.DataStructures;
using AudioVisual.Utils;

namespace AudioVisual.Visualizer
{
    public class CircularWaveVisualizer : VisualizerAbstract
    {
        private const double RadiusScalingFactor = 0.3;

        protected override List<List<Line>> DrawLines(List<List<double>> visualizationData)
        {
            var count = visualizationData.Count;

            var visualizableList = new List<List<Line>>(count);

            for (var i = 0; i < count; i++)
            {
                // Line information
                var lineData = visualizationData[i]
                    .NormalizeValues()
                    .SubSample(Constants.SegmentCount);

                var lines = ToCircle(lineData);

                visualizableList.Add(lines);
            }

            return visualizableList;
        }

        protected override List<DrawingLayoutData> GetLayouts(int layoutCount, double width, double height)
        {
            var drawingLayouts = new List<DrawingLayoutData>(layoutCount);

            // Grid
            var gridCount = Math.Ceiling(Math.Sqrt(layoutCount));
            var rowDistanceBetweenCircles = height / (gridCount + 1);
            var columnDistanceBetweenCircles = width / (gridCount + 1);

            var iLayout = 0;

            var hues = FrequencyToColorMapper.GetListOfHues(layoutCount);

            for (var row = 0; row < gridCount && iLayout < layoutCount; row++)
            {
                var offsetY = rowDistanceBetweenCircles * (row + 1);

                for (var column = 0; column < gridCount && iLayout < layoutCount; column++)
                {
                    // Color information
                    var hue = hues[iLayout];
                    var color = FrequencyToColorMapper.ColorFromHsv(hue, 1, 1);
                    var brush = new SolidColorBrush(color);

                    // Position
                    var offsetX = columnDistanceBetweenCircles * (column + 1);
                    var radius = Math.Min(rowDistanceBetweenCircles, columnDistanceBetweenCircles) * RadiusScalingFactor;

                    var center = new Point(offsetX, offsetY);

                    var layout = new DrawingLayoutData(center, radius, radius, brush);

                    drawingLayouts.Add(layout);

                    iLayout++;
                }
            }

            return drawingLayouts;
        }


        private static List<Line> ToCircle(IReadOnlyList<double> lineData)
        {
            var lines = new List<Line>();

            var maxFluctuation = lineData.Max() * 0.5;

            var fluctuationFirst = 1 + maxFluctuation * lineData[0];
            var pointFirst = new Point(fluctuationFirst, 0).PolarToCartesian();

            var x1 = pointFirst.X;
            var y1 = pointFirst.Y;

            var angleStep = (Math.PI * 2) / (lineData.Count);

            for (var iSegment = 1; iSegment <= lineData.Count; iSegment++)
            {
                var theta = angleStep * iSegment;

                if (iSegment != (lineData.Count))
                {

                    var fluctuation = 1 + maxFluctuation * lineData[iSegment];
                    var point = new Point(fluctuation, theta).PolarToCartesian();

                    var line = new Line
                    {
                        X1 = x1,
                        Y1 = y1,
                        X2 = point.X,
                        Y2 = point.Y
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
                        Y2 = point.Y
                    };

                    lines.Add(line);
                    break;
                }
            }

            return lines;
        }
    }
}
