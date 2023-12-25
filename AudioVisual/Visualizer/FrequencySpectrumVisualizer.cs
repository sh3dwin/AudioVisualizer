using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using AudioVisual.Utils;

namespace AudioVisual.Visualizer
{
    public class FrequencySpectrumVisualizer : IDisposable
    {
        private readonly Canvas _canvas;

        private readonly List<LinkedList<double>> _previousValues;

        public FrequencySpectrumVisualizer(Canvas canvas)
        {
            _canvas = canvas;
            _previousValues = new List<LinkedList<double>>();
        }
        public Canvas Draw(List<double> amplitudes)
        {
            ClearCanvas();

            amplitudes = MathUtils.NormalizeValues(amplitudes);

            var windowHeight = (int)_canvas.ActualHeight;
            var windowWidth = (int)_canvas.ActualWidth;

            if (windowHeight == 0 || windowWidth == 0)
                return _canvas;

            var segmentWidth = (double)windowWidth / amplitudes.Count;

            var hues = FrequencyToColorMapper.GetListOfHues(amplitudes.Count);

            for (var i = 0; i < amplitudes.Count; i ++)
            {
                //var height = GetSmoothedValue(amplitudes[i], i);
                var height = _canvas.ActualHeight * Math.Abs(amplitudes[i]);
                var color = FrequencyToColorMapper.ColorFromHSV(hues[i], 1, 1);
                var rect = SoundWaveUtils.CreateRectangle(height, segmentWidth, new SolidColorBrush(color));

                _canvas.Children.Add(rect);

                Canvas.SetTop(rect, (windowHeight) - (amplitudes[i] >= 0 ? 1 : 0) * rect.Height);
                Canvas.SetLeft(rect, i * rect.Width);
            }

            return _canvas;
        }

        private double GetSmoothedValue(double amplitude, int index)
        {
            var currentAmplitude = _canvas.ActualHeight * amplitude * 10;
            if (_previousValues.Count > index)
            {
                _previousValues[index].AddFirst(currentAmplitude);
                _previousValues[index].RemoveLast();
            }
            else
            {
                _previousValues.Add(new LinkedList<double>());
                for (int j = 0; j < Constants.SmoothingFactor; j++)
                    _previousValues[index].AddFirst(0);
            }
            var sumWithPrevious = MathUtils.AddLinkedListElements(_previousValues[index]);

            return sumWithPrevious;
        }
        private void ClearCanvas()
        {
            _canvas.Background = new SolidColorBrush(Color.FromRgb(10, 10, 10));
            _canvas.Children.Clear();
        }

        public void Dispose()
        {
            _previousValues.Clear();
        }
    }
}