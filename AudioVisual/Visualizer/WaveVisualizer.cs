using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using AudioVisual.DataStructures;
using AudioVisual.Utils;

namespace AudioVisual.Visualizer
{
    public class WaveVisualizer : VisualizerAbstract
    {
        protected override List<List<Line>> DrawLines(List<List<double>> visualizationData)
        {
            var waveCount = visualizationData.Count;

            var normalizedVisualizationData = visualizationData.NormalizeValues();

            var visualizableList = new List<List<Line>>(waveCount);

            for (var iWave = 0; iWave < waveCount; iWave++)
            {
                // Line information
                var lineData = normalizedVisualizationData[iWave]
                    .SubSample(Constants.SegmentCount);

                var lines = ToLines(lineData);

                visualizableList.Add(lines);
            }

            return visualizableList;
        }

        protected override List<DrawingLayoutData> GetLayouts(int layoutCount, double width, double height)
        {
            var drawingLayouts = new List<DrawingLayoutData>(layoutCount);

            // Position
            var scaleX = width / Constants.SegmentCount;
            var scaleY = (height / (1 + layoutCount)) / 2;

            var yStep = (int)height / (1 + layoutCount);

            for (var iLayout = 0; iLayout < layoutCount; iLayout++)
            {
                const double startX = 0.0;
                var startY = (iLayout + 1.0) * yStep;
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

        private static List<Line> ToLines(IReadOnlyList<double> lineData)
        {
            var y1 = lineData[0];

            var lines = new List<Line>(Constants.SegmentCount);

            for (var iSegment = 1; iSegment < Constants.SegmentCount; iSegment++)
            {
                var line = new Line
                {
                    X1 = iSegment,
                    Y1 = y1,
                    X2 = (iSegment + 1),
                    Y2 = lineData[iSegment],
                };
                lines.Add(line);

                y1 = line.Y2;
            }

            return lines;
        }
    }
}
