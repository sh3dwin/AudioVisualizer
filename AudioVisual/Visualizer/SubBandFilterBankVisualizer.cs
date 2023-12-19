using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using AudioVisual.DataStructures;

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

        public Canvas Draw(List<BandPassFilteredWave> subBandFilterBank)
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
                var currentBandPassedWave = subBandFilterBank[iBandPass];
                var bandPassValues = currentBandPassedWave.Values;
                var hue = currentBandPassedWave.GetAveragedFrequency() * (360.0 / currentBandPassedWave.SampleRate);
                var color = FrequencyToColorMapper.ColorFromHSV(hue, 1, 1);
                var brush = new SolidColorBrush(color);
                var offset = (iBandPass + 1) * yStep;
                var wave = DrawBandPassWave(bandPassValues, brush, offset);
                canvasLines.AddRange(wave);
            }

            foreach (var line in canvasLines)
            {
                _canvas.Children.Add(line);
            }

            return _canvas;
        }

        private List<Line> DrawBandPassWave(IReadOnlyList<double> bandPassValues, SolidColorBrush color, double offset)
        {
            var intervalLength = _canvas.ActualWidth / bandPassValues.Count;

            var y1 = offset;

            var lines = new List<Line>(bandPassValues.Count);
            for (var i = 0; i < bandPassValues.Count; i++)
            {
                var line = new Line();
                line.X1 = i * intervalLength;
                line.Y1 = y1;
                line.X2 = (i + 1) * intervalLength;
                line.Y2 = bandPassValues[i] * AmplitudeScalingFactor + offset;
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
