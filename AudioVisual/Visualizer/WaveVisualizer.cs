using NAudio.Wave;
using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using AudioVisual.Visualizer;
using NAudio.Dsp;

namespace AudioVisual
{
    public class WaveVisualizer : IDisposable, IVisualizer
    {
        private WaveBuffer _buffer;
        private Canvas _canvas;

        private int _samples = 200;
        private int _sampleRate = 0;
        private int _sampleSize = 0;

        private Complex[] _fftValues;

        private int amplitudeScalingFactor = 700;

        private double denom = 1 / 44000.0;

        private WasapiLoopbackCapture _capture;

        private int M = 15;

        private double _binCount;
        private double _binSize;

        private double[] _finalWave;

        public WaveVisualizer(Canvas canvas)
        {
            _canvas = canvas;
            Load();
        }

        public void Load()
        {
            // start audio capture
            _capture = new WasapiLoopbackCapture();

            _sampleRate = _capture.WaveFormat.SampleRate;

            _capture.DataAvailable += DataAvailable;

            _capture.RecordingStopped += (s, a) => { _capture.Dispose(); };

            _capture.StartRecording();
        }

        public void DataAvailable(object sender, WaveInEventArgs e)
        {
            _buffer = new WaveBuffer(e.Buffer); // save the _buffer in the class variable
        }

        public Canvas Draw()
        {
            ClearCanvas();

            // Get current width and height in case of resize
            int windowHeight = (int)_canvas.ActualHeight;
            int windowWidth = (int)_canvas.ActualWidth;

            if (windowHeight == 0 || windowWidth == 0)
                return _canvas;

            if (_buffer == null)
            {
                throw new Exception("No buffer available");
            }
            // fft
            _fftValues = SoundWaveUtils.CreateAndInitializeComplexArray(this._buffer, M);

            FastFourierTransform.FFT(true, M, _fftValues);
            // fft
            _sampleSize = (int)Math.Ceiling(_fftValues.Length * 0.5);
            _finalWave = new double[_sampleSize];

            // The number of elements which contain information about the first count * frequency_step frequencies

            DrawSoundWave(0, 100, new SolidColorBrush(Color.FromScRgb(1f, 1f, 0f, 0f)), 1);
            DrawSoundWave(100, 500, new SolidColorBrush(Color.FromScRgb(1f, 1f, 1f, 0f)), 2);
            DrawSoundWave(500, 1500, new SolidColorBrush(Color.FromScRgb(1f, 0f, 1f, 1f)), 3);
            DrawSoundWave(1500, 5000, new SolidColorBrush(Color.FromScRgb(1f, 1f, 0f, 1f)), 4);
            DrawSoundWave(5000, 20000, new SolidColorBrush(Color.FromScRgb(1f, 0f, 0f, 1f)), 5);
            DrawWholeSoundWave(new SolidColorBrush(Color.FromScRgb(1f, 1f, 1f, 1f)));


            return this._canvas;
        }

        private void DrawSoundWave(double minFrequency, double maxFrequency, SolidColorBrush color, double yOffset)
        {
            var size = (int)_canvas.ActualWidth / (double)_samples;
            var midLine = (int)_canvas.ActualHeight * 1/6;
            var offset = yOffset * midLine;
            var y1 = (double)offset;
            for (var i = 0; i < _samples; i++)
            {
                var sumOfAmplitudes = SumAllAmplitudesBetweenFrequencies(minFrequency, maxFrequency, i);

                var line = new Line();
                line.X1 = i * size;
                line.Y1 = y1;
                line.X2 = (i + 1) * size;
                line.Y2 = sumOfAmplitudes * amplitudeScalingFactor + offset;
                line.StrokeThickness = 3;
                y1 = line.Y2;
                line.Stroke = color;

                _canvas.Children.Add(line);
            }
        }

        private void DrawWholeSoundWave(SolidColorBrush color)
        {
            var size = (int)_canvas.ActualWidth / (double)_samples;
            var midLine = (int)_canvas.ActualHeight * 1/6;
            var offset = 6 * midLine;
            var y1 = (double)offset;
            for (var i = 0; i < _samples; i++)
            {
                var line = new Line();
                line.X1 = i * size;
                line.Y1 = y1;
                line.X2 = (i + 1) * size;
                line.Y2 = _finalWave[i] * amplitudeScalingFactor + offset;
                line.StrokeThickness = 3;
                y1 = line.Y2;
                line.Stroke = color;

                _canvas.Children.Add(line);
            }
        }
        private double SumAllAmplitudesBetweenFrequencies(double minFrequency, double maxFrequency, int index)
        {
            _binCount = _sampleSize;
            // frequency change from bin to bin
            _binSize = _sampleRate / _binCount;
            var size = (int)_canvas.ActualWidth / (double)_samples;
            double frequency = minFrequency;
            var minFrequencyBin = minFrequency / _binSize;
            var maxFrequencyBin = maxFrequency / _binSize;
            var sumOfAmplitudes = 0.0;
            for (int i = (int)minFrequencyBin; i < maxFrequencyBin; i++)
            {
                //Console.WriteLine($"Frequency is {frequency} at bin {i}, min frequency at {minFrequencyBin}, maximum at {maxFrequencyBin}");
                sumOfAmplitudes += _fftValues[i].X * Math.Sin((2 * Math.PI * index * size * frequency + _fftValues[i].Y) * denom);
                frequency += _binSize;
            }

            _finalWave[index] += sumOfAmplitudes;

            return sumOfAmplitudes;
        }

        private void ClearCanvas()
        {
            _canvas.Background = new SolidColorBrush(Color.FromRgb(10, 10, 10));
            _canvas.Children.Clear();
        }



        public void Dispose()
        {
            _capture.DataAvailable -= DataAvailable;

            _capture.StopRecording();

            _capture.Dispose();
        }
    }
}
