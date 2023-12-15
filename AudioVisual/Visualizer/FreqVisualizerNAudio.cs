using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using AudioVisual.Visualizer;
using NAudio.Dsp;

namespace AudioVisual
{
    /*
     * Visualizer using frequencies
     */
    class FreqVisualizerNAudio : IDisposable
    {
        private Canvas canvas;


        private List<LinkedList<double>> _previousValues;
        private int _smoothingFactor = 10;

        public FreqVisualizerNAudio(Canvas canvas)
        {
            this.canvas = canvas;
            _previousValues = new List<LinkedList<double>>();
        }
        public Canvas Draw(List<double> amplitudes, List<double> hues)
        {
            ClearCanvas();

            // Get current width and height in case of resize
            int windowHeight = (int)canvas.ActualHeight;
            int windowWidth = (int)canvas.ActualWidth;

            if (windowHeight == 0 || windowWidth == 0)
                return canvas;

            double width = (double)windowWidth / amplitudes.Count;

            for (int i = 0; i < amplitudes.Count; i++)
            {
                var currentAmplitude = Math.Abs(windowHeight * amplitudes[i]) * 10;
                if (_previousValues.Count > i)
                {
                    _previousValues[i].AddFirst(currentAmplitude);
                    _previousValues[i].RemoveLast();
                }
                else
                {
                    _previousValues.Add(new LinkedList<double>());
                    for (int j = 0; j < _smoothingFactor; j++)
                        _previousValues[i].AddFirst(0);
                }
                var sumWithPrevious = MathUtils.AddLinkedListElements(_previousValues[i]);
                var height = sumWithPrevious;
                var color = FrequencyToColorMapper.ColorFromHSV(hues[i], 1, 1);
                var rect = SoundWaveUtils.CreateRectangle(height, width, new SolidColorBrush(color));

                canvas.Children.Add(rect);
                // position the rectangle on the canvas
                Canvas.SetTop(rect, windowHeight - rect.Height);
                Canvas.SetLeft(rect, i * rect.Width);
            }

            return this.canvas;
        }
        private void ClearCanvas()
        {
            canvas.Background = new SolidColorBrush(Color.FromRgb(10, 10, 10));
            canvas.Children.Clear();
        }
        public Canvas Draw(Complex[] frequencySpectrum)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            
        }
    }
}