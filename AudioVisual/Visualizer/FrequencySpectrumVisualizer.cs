using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using AudioVisual.Utils;

namespace AudioVisual.Visualizer
{
    public class FrequencySpectrumVisualizer : IDisposable
    {

        private readonly List<LinkedList<double>> _previousValues;

        public FrequencySpectrumVisualizer()
        {
            _previousValues = new List<LinkedList<double>>();
        }
        public Canvas Draw(Canvas canvas, List<double> amplitudes)
        {
            canvas.Clear();

            amplitudes = MathUtils.NormalizeValues(amplitudes);

            var windowHeight = (int)canvas.ActualHeight;
            var windowWidth = (int)canvas.ActualWidth;

            if (windowHeight == 0 || windowWidth == 0)
                return canvas;

            var segmentWidth = (double)windowWidth / amplitudes.Count;

            var hues = FrequencyToColorMapper.GetListOfHues(amplitudes.Count);

            for (var i = 0; i < amplitudes.Count; i ++)
            {
                var height = GetSmoothedValue(amplitudes[i], i, windowHeight);
                var color = FrequencyToColorMapper.ColorFromHsv(hues[i], 1, 1);
                var rect = SoundWaveUtils.CreateRectangle(height, segmentWidth, new SolidColorBrush(color));

                canvas.Children.Add(rect);

                Canvas.SetTop(rect, windowHeight - rect.Height);
                Canvas.SetLeft(rect, i * rect.Width);
            }

            return canvas;
        }

        private double GetSmoothedValue(double amplitude, int index, double canvasHeight)
        {
            var currentAmplitude = canvasHeight * amplitude;
            if (_previousValues.Count > index)
            {
                _previousValues[index].AddFirst(currentAmplitude);
                _previousValues[index].RemoveLast();
            }
            else
            {
                _previousValues.Add(new LinkedList<double>());
                for (var j = 0; j < Constants.SmoothingFactor; j++)
                    _previousValues[index].AddFirst(0);
            }
            var sumWithPrevious = MathUtils.AddLinkedListElements(_previousValues[index]);

            return sumWithPrevious;
        }

        public void Dispose()
        {
            _previousValues.Clear();
        }
    }
}