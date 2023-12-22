using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
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
                var filter = subBandFilterBank[iBandPass];
                var wave = FourierTransformAnalyzer.ToWave(filter, Constants.PowerOfTwo);
                var hue = filter.GetAveragedFrequency() * (360.0 / (filter.SampleRate / 2.0));
                var color = FrequencyToColorMapper.ColorFromHSV(hue, 1, 1);
                var brush = new SolidColorBrush(color);
                var offset = (iBandPass + 1) * yStep;
                var visualizedWave = DrawBandPassWave(wave, brush, offset);
                canvasLines.AddRange(visualizedWave);
            }

            foreach (var line in canvasLines)
            {
                _canvas.Children.Add(line);
            }

            return _canvas;
        }

        private List<Line> DrawBandPassWave(IReadOnlyList<float> bandPassValues, SolidColorBrush color, double offset)
        {
            var intervalLength = _canvas.ActualWidth / Constants.SegmentCount;
            var step = bandPassValues.Count / Constants.SegmentCount; 

            var y1 = offset;

            var lines = new List<Line>(Constants.SegmentCount);
            for (var i = 0; i < Constants.SegmentCount; i++)
            {
                var line = new Line();
                line.X1 = i * intervalLength;
                line.Y1 = y1;
                line.X2 = (i + 1) * intervalLength;
                line.Y2 = bandPassValues[i * step] * AmplitudeScalingFactor + offset;
                line.StrokeThickness = 3;
                y1 = line.Y2;
                line.Stroke = color;

                lines.Add(line);
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
