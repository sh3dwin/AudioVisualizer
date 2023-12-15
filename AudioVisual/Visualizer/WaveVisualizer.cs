using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using AudioVisual.Visualizer;

namespace AudioVisual
{
    public class WaveVisualizer : IDisposable
    {
        private readonly Canvas _canvas;

        private int _samples;
        private double _segmentSize;

        private int amplitudeScalingFactor = 400;

        public WaveVisualizer(Canvas canvas)
        {
            _canvas = canvas;
        }
        public Canvas Draw(List<List<double>> summedWaves, List<double> hues)
        {
            ClearCanvas();

            _samples = summedWaves[0].Count;

            _segmentSize = _canvas.ActualWidth / _samples;

            var numberOfWaves = summedWaves.Count;
            var canvasLines = new List<Line>(_samples * (numberOfWaves) * 2);

            // Distance from each separate wave line
            var yStep = (int)_canvas.ActualHeight / (1 + numberOfWaves);

            for (int partitionIndex = 0; partitionIndex < numberOfWaves; partitionIndex++)
            {
                var summedValues = summedWaves[partitionIndex];
                var color = FrequencyToColorMapper.ColorFromHSV(hues[partitionIndex], 1, 1);
                var brush = new SolidColorBrush(color);
                var offset = (partitionIndex + 1) * yStep;
                var wave = DrawSoundWave(summedValues, brush, offset);
                canvasLines.AddRange(wave);
            }

            foreach (var line in canvasLines)
            {
                _canvas.Children.Add(line);
            }

            return this._canvas;
        }
        private List<Line> DrawSoundWave(IReadOnlyList<double> summedValues, SolidColorBrush color, double offset)
        {
            
            var y1 = offset;

            var lines = new List<Line>(summedValues.Count);
            for (var i = 0; i < summedValues.Count; i++)
            {
                var line = new Line();
                line.X1 = i * _segmentSize;
                line.Y1 = y1;
                line.X2 = (i + 1) * _segmentSize;
                line.Y2 = summedValues[i] * amplitudeScalingFactor + offset;
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
