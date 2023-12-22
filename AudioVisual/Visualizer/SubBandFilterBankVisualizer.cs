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
    public class SubBandFilterBankVisualizer : IDisposable
    {
        private readonly Canvas _canvas;

        private const int AmplitudeScalingFactor = 400;

        public SubBandFilterBankVisualizer(Canvas canvas)
        {
            _canvas = canvas;
        }

        public Canvas Draw(List<FrequencyFilter> subBandFilterBank)
        {
            ClearCanvas();

            if (subBandFilterBank[0] is null)
            {
                return _canvas;
            }

            var bandPassCount = subBandFilterBank.Count;
            var canvasLines = new List<Line>(subBandFilterBank[0].Values.Count * (bandPassCount) * 2);

            // Distance from each separate wave line
            var yStep = (int)_canvas.ActualHeight / (1 + bandPassCount);

            for (int iBandPass = 0; iBandPass < bandPassCount; iBandPass++)
            {
                // Wave information
                var filter = subBandFilterBank[iBandPass];
                var wave = FourierTransformAnalyzer.ToWave(filter, Constants.PowerOfTwo);

                // Color information
                var hue = filter.GetAveragedFrequency() * (360.0 / (Constants.SampleRate / 2.0));
                var color = FrequencyToColorMapper.ColorFromHSV(hue, 1, 1);
                var brush = new SolidColorBrush(color);

                // Position
                var offset = (iBandPass + 1) * yStep;

                var visualizedWave = DrawWave(wave, brush, offset);
                canvasLines.AddRange(visualizedWave);
            }

            foreach (var line in canvasLines)
            {
                _canvas.Children.Add(line);
            }

            return _canvas;
        }

        private List<Line> DrawWave(IReadOnlyList<float> wave, SolidColorBrush color, double offset)
        {
            var intervalLength = _canvas.ActualWidth / Constants.SegmentCount;
            var step = wave.Count / Constants.SegmentCount; 

            var y1 = offset;

            var lines = new List<Line>(Constants.SegmentCount);

            for (var iSegment = 0; iSegment < Constants.SegmentCount; iSegment++)
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

        private void ClearCanvas()
        {
            _canvas.Background = new SolidColorBrush(Color.FromRgb(10, 10, 10));
            _canvas.Children.Clear();
        }

        public void Dispose()
        {
        }
    }
}
