using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using AudioVisual.Analysis;
using AudioVisual.DataStructures;
using AudioVisual.Utils;

namespace AudioVisual.Visualizer
{
    public class FilterBankVisualizer : IDisposable
    {

        private const int AmplitudeScalingFactor = 400;

        public Canvas Draw(Canvas canvas, List<FrequencyFilter> subBandFilterBank)
        {
            canvas.Clear();

            if (subBandFilterBank[0] is null)
            {
                return canvas;
            }

            var bandPassCount = subBandFilterBank.Count;
            var canvasLines = new List<Line>(subBandFilterBank[0].Values.Count * (bandPassCount) * 2);

            var hues = FrequencyToColorMapper.GetListOfHues(bandPassCount);

            // Distance from each separate wave line
            var yStep = (int)canvas.ActualHeight / (1 + bandPassCount);
            var intervalLength = canvas.ActualWidth / Constants.SegmentCount;

            for (int iBandPass = 0; iBandPass < bandPassCount; iBandPass++)
            {
                // Wave information
                var filter = subBandFilterBank[iBandPass];
                var wave = FourierTransformAnalyzer.ToWave(filter, Constants.PowerOfTwo);

                // Color information
                var hue = hues[iBandPass];
                var color = FrequencyToColorMapper.ColorFromHsv(hue, 1, 1);
                var brush = new SolidColorBrush(color);

                // Position
                var offset = (iBandPass + 1) * yStep;

                var visualizedWave = DrawWave(wave, brush, offset, intervalLength);
                canvasLines.AddRange(visualizedWave);
            }

            foreach (var line in canvasLines)
            {
                canvas.Children.Add(line);
            }

            return canvas;
        }

        private List<Line> DrawWave(IReadOnlyList<double> wave, SolidColorBrush color, double offset, double intervalLength)
        {
            
            var step = wave.Count / Constants.SegmentCount; 

            var y1 = offset + wave[0] * AmplitudeScalingFactor;

            var lines = new List<Line>(Constants.SegmentCount);

            for (var iSegment = 1; iSegment < Constants.SegmentCount; iSegment++)
            {
                var line = new Line
                {
                    X1 = iSegment * intervalLength,
                    Y1 = y1,
                    X2 = (iSegment + 1) * intervalLength,
                    Y2 = wave[iSegment * step] * AmplitudeScalingFactor + offset,
                    Stroke = color,
                    StrokeThickness = 3
                };
                lines.Add(line);

                y1 = line.Y2;
            }

            return lines;
        }

        public void Dispose()
        {
        }
    }
}
