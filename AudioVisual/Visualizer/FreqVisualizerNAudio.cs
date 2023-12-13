using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using AudioVisual;
using AudioVisual.Visualizer;
using NAudio.Dsp;

namespace AudioVisual
{
    enum SmoothType
    {
        horizontal,
        vertical,
        both
    }
    /*
     * Visualizer using frequencies
     */
    class FreqVisualizerNAudio : IDisposable, IVisualizer
    {
        private WaveBuffer buffer;
        private Canvas canvas;

        private static int vertical_smoothness = 3;
        private static int horizontal_smoothness = 1;
        private float size = 10;
        private int _nSplits = 200;

        private int vis_mode = 0;

        private static SmoothType smoothType = SmoothType.both;

        private List<Complex[]> smooth = new List<Complex[]>();

        private List<LinkedList<double>> _previousValues;
        private int _smoothingFactor = 10;

        private List<List<Complex>> _aggregatedList = null;

        private List<int> _splits = null;

        private double pre_value = 0;

        private double count = 64;

        private double _lstR = 0;
        private double _lstG = 0;
        private double _lstB = 0;

        private WasapiLoopbackCapture _capture;

        private int M = 15;

        public FreqVisualizerNAudio(Canvas canvas)
        {
            this.canvas = canvas;
            int count = (int)Math.Pow(2, M - 4);
            _splits = MathUtils.SplitIntoNGeometricSeries(_nSplits, count);
            _previousValues = new List<LinkedList<double>>();
            for (int i = 0; i < _nSplits; i++)
            {
                _previousValues.Add(new LinkedList<double>());
                for (var j = 0; j < _smoothingFactor; j++)
                {
                    _previousValues[i].AddFirst(0);
                }
            }
            Load();
        }

        public void Load()
        {
            // start audio capture
            _capture = new WasapiLoopbackCapture();

            _capture.DataAvailable += DataAvailable;

            _capture.RecordingStopped += (s, a) => { _capture.Dispose(); };

            _capture.StartRecording();
        }

        public void DataAvailable(object sender, WaveInEventArgs e)
        {
            buffer = new WaveBuffer(e.Buffer); // save the buffer in the class variable
        }

        public Canvas Draw()
        {
            ClearCanvas();

            // Get current width and height in case of resize
            int windowHeight = (int)canvas.ActualHeight;
            int windowWidth = (int)canvas.ActualWidth;

            if (windowHeight == 0 || windowWidth == 0)
                return canvas;

            if (buffer == null)
            {
                throw new Exception("No buffer available");
            }
            // fft
            var values = SoundWaveUtils.CreateAndInitializeComplexArray(this.buffer, M);

            FastFourierTransform.FFT(true, M, values);
            // fft

            // The number of elements which contain information about the first count * frequency_step frequencies
            int count = (int)Math.Pow(2, M - 4);
            if (_aggregatedList is not null)
                _aggregatedList = SoundWaveUtils.FillAggregatedList(_aggregatedList, _splits, values, count);
            else
            {
                _aggregatedList = SoundWaveUtils.Aggregate(values, _nSplits, count);
            }
            double width = (float)windowWidth / _aggregatedList.Count;

            for (int i = 0; i < _aggregatedList.Count; i++)
            {
                var currentAmplitude = Math.Abs(windowHeight * SoundWaveUtils.GetTopNOrTop5Average(_aggregatedList[i])) * 10 * Math.Log(i + 2);
                _previousValues[i].AddFirst(currentAmplitude);
                _previousValues[i].RemoveLast();
                var sumWithPrevious = MathUtils.AddLinkedListElements(_previousValues[i]);
                var height = sumWithPrevious;
                var rect = SoundWaveUtils.CreateRectangle(height, width, GetBrushColor(_aggregatedList));

                canvas.Children.Add(rect);
                // position the rectangle on the canvas
                Canvas.SetTop(rect, windowHeight - rect.Height);
                Canvas.SetLeft(rect, i * rect.Width);
            }

            return this.canvas;
        }



        public double BothSmooth(int i)
        {
            var s = smooth.ToArray();

            double value = 0;

            for (int h = Math.Max(i - horizontal_smoothness, 0); h < Math.Min(i + horizontal_smoothness, 64); h++)
                value += vSmooth(h, s);

            return value / ((horizontal_smoothness + 1) * 2);
        }

        public double vSmooth(int i, Complex[][] s)
        {
            double value = 0;

            for (int v = 0; v < s.Length; v++)
                value += Math.Abs(s[v] != null ? MathUtils.Magnitude(s[v][i]) : 0.0);

            return value / s.Length;
        }

        private SolidColorBrush GetBrushColor(List<List<Complex>> aggregatedList)
        {
            var r = SoundWaveUtils.SumOfLowFrequencyAmplitudes(aggregatedList, _nSplits);
            var g = SoundWaveUtils.SumOfMidFrequencyAmplitudes(aggregatedList, _nSplits);
            var b = SoundWaveUtils.SumOfHighFrequencyAmplitudes(aggregatedList, _nSplits);

            var sumDenom = 1 / (r + g + b);
            var rWeighted = r * sumDenom;
            var gWeighted = g * sumDenom;
            var bWeighted = b * sumDenom;
            var max = Math.Max(rWeighted, Math.Max(gWeighted, bWeighted));
            var finalR = r / max;
            var finalG = g / max;
            var finalB = b / max;
            //var brush = new SolidColorBrush(Color.FromScRgb(1f, 
            //    (float)(r * + _lstR) / 2,
            //    (float)(g + _lstG) / 2,
            //    (float)(b + _lstB) / 2));
            var brush = new SolidColorBrush(Color.FromScRgb(1f, (float)finalR, (float)finalG, (float)finalB));
            _lstR = r;
            _lstG = g;
            _lstB = b;
            return brush;
        }

        private void ClearCanvas()
        {
            canvas.Background = new SolidColorBrush(Color.FromRgb(10, 10, 10));
            canvas.Children.Clear();
        }

        

        public void Dispose()
        {
            _capture.DataAvailable -= DataAvailable;

            _capture.StopRecording();

            _capture.Dispose();
        }
    }
}