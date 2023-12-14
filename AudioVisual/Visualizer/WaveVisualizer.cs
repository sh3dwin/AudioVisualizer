using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using AudioVisual.Visualizer;
using NAudio.Dsp;

namespace AudioVisual
{
    public class WaveVisualizer : IDisposable, IVisualizer
    {
        private Canvas _canvas;

        private int _samples = 200;
        private int _sampleRate;
        private int _sampleSize;
        private int _maxFrequency;
        private double _segmentSize;

        private Complex[] _fftValues;

        private int amplitudeScalingFactor = 400;

        private double denom;

        private double _binCount;
        private double _binSize;

        private double[] _finalWave;

        private int _wavePartitionsNumber = 3;
        private List<int> _wavePartitionsSplits;

        public WaveVisualizer(Canvas canvas, int sampleRate)
        {
            _canvas = canvas;
            _sampleRate = sampleRate;
            denom = 1.0 / sampleRate;
            _maxFrequency = (int)(_sampleRate * 0.5);
            _wavePartitionsSplits = MathUtils.SplitIntoNGeometricSeries(_wavePartitionsNumber, _maxFrequency);
        }

        public Canvas Draw(Complex[] frequencySpectrum)
        {
            ClearCanvas();

            // Get current width and height in case of resize
            int windowHeight = (int)_canvas.ActualHeight;
            int windowWidth = (int)_canvas.ActualWidth;

            _segmentSize = windowWidth / (double)_samples;

            if (windowHeight == 0 || windowWidth == 0)
                return _canvas;

            _fftValues = frequencySpectrum;
            _sampleSize = (int)Math.Ceiling((double)_fftValues.Length);
            _finalWave = new double[_sampleSize];

            var splitLowerBound = 0;
            for (int splitIndex = 0; splitIndex < _wavePartitionsNumber; splitIndex++)
            {
                
                DrawSoundWave(splitLowerBound,
                    splitLowerBound + _wavePartitionsSplits[splitIndex],
                    new SolidColorBrush(Color.FromScRgb(1f, 1f, 0f, 0f)),
                    splitIndex + 1);
                splitLowerBound += _wavePartitionsSplits[splitIndex];
            }
            DrawWholeSoundWave(new SolidColorBrush(Color.FromScRgb(1f, 1f, 1f, 1f)));

            return this._canvas;
        }

        public void ChangeWavePartitions(int partitionsNumber)
        {
            _wavePartitionsNumber = partitionsNumber;
            _wavePartitionsSplits = MathUtils.SplitIntoNGeometricSeries(_wavePartitionsNumber, _maxFrequency);
        }

        private void DrawSoundWave(double lowerFrequencyBoundary, double upperFrequencyBoundary, SolidColorBrush color, double yOffset)
        {
            var yStep= (int)_canvas.ActualHeight / (2 + _wavePartitionsNumber);
            var offset = yOffset * yStep;
            var y1 = (double)offset;
            for (var i = 0; i < _samples; i++)
            {
                var sumOfAmplitudes = SumAllAmplitudesBetweenFrequencies(lowerFrequencyBoundary, upperFrequencyBoundary, i);

                var line = new Line();
                line.X1 = i * _segmentSize;
                line.Y1 = y1;
                line.X2 = (i + 1) * _segmentSize;
                line.Y2 = sumOfAmplitudes * amplitudeScalingFactor + offset;
                line.StrokeThickness = 3;
                y1 = line.Y2;
                line.Stroke = color;

                _canvas.Children.Add(line);
            }
        }

        private void DrawWholeSoundWave(SolidColorBrush color)
        {
            var yStep = (int)_canvas.ActualHeight / (2 + _wavePartitionsNumber);
            var offset = (1 + _wavePartitionsNumber) * yStep;
            var y1 = (double)offset;
            for (var i = 0; i < _samples; i++)
            {
                var line = new Line();
                line.X1 = i * _segmentSize;
                line.Y1 = y1;
                line.X2 = (i + 1) * _segmentSize;
                line.Y2 = _finalWave[i] * amplitudeScalingFactor + offset;
                line.StrokeThickness = 3;
                y1 = line.Y2;
                line.Stroke = color;

                _canvas.Children.Add(line);
            }
        }
        private double SumAllAmplitudesBetweenFrequencies(double lowerFrequencyBoundary, double upperFrequencyBoundary, int index)
        {
            _binCount = _sampleSize;
            // frequency change from bin to bin
            _binSize = _sampleRate / (_binCount * 2);
            var size = (int)_canvas.ActualWidth / (double)_samples;
            double frequency = lowerFrequencyBoundary;
            var minFrequencyBin = lowerFrequencyBoundary / _binSize;
            var maxFrequencyBin = upperFrequencyBoundary / _binSize;
            var sumOfAmplitudes = 0.0;
            var constant = 2 * Math.PI * index * size;
            for (int i = (int)minFrequencyBin; i < maxFrequencyBin && i < _sampleSize; i++)
            {
                //Console.WriteLine($"Frequency is {frequency} at bin {i}, min frequency at {minFrequencyBin}, maximum at {maxFrequencyBin}");
                sumOfAmplitudes += _fftValues[i].X * Math.Sin((constant * frequency + MathUtils.Argument(_fftValues[i])) * denom);
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
        }
    }
}
