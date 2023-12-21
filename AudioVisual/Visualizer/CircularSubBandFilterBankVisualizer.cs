using AudioVisual.DataStructures;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using AudioVisual.Utils;
using NAudio.Dsp;

namespace AudioVisual.Visualizer
{
    public class CircularSubBandFilterBankVisualizer : IDisposable
    {
        private readonly Canvas _canvas;

        private const double RadiusScalingFactor = 0.3;

        public int InterpolationCount = 5;

        public CircularSubBandFilterBankVisualizer(Canvas canvas)
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

            // Position of circle centers
            var gridCount = Math.Ceiling(Math.Sqrt(subBandFilterBank.Count));
            var rowDistanceBetweenCircles = _canvas.ActualHeight / (gridCount + 1);
            var columnDistanceBetweenCircles = _canvas.ActualWidth / (gridCount + 1);

            var iBandPass = 0;

            var colorValues = GetWaveColorSignificance(subBandFilterBank);

            for (var row = 0; row < gridCount && iBandPass < bandPassCount; row++)
            {
                var offsetY = rowDistanceBetweenCircles * (row + 1);

                for (var column = 0; column < gridCount && iBandPass < bandPassCount; column++)
                {
                    var currentBandPassedWave = subBandFilterBank[iBandPass];
                    var bandPassValues = currentBandPassedWave.Values;
                    var hue = currentBandPassedWave.GetAveragedFrequency() * (360.0 / (currentBandPassedWave.SampleRate / 8.0));
                    var saturation = colorValues[iBandPass];
                    var color = FrequencyToColorMapper.ColorFromHSV(hue, 1, Math.Max(saturation, 0.3));
                    var brush = new SolidColorBrush(color);
                    var offsetX = columnDistanceBetweenCircles * (column + 1);
                    var radius = Math.Min(rowDistanceBetweenCircles, columnDistanceBetweenCircles) * RadiusScalingFactor;
                    var wave = DrawCircularBandPassedWave(bandPassValues, brush, offsetX, offsetY, radius);
                    canvasLines.AddRange(wave);

                    iBandPass++;
                }
            }

            foreach (var line in canvasLines)
            {
                _canvas.Children.Add(line);
            }

            return _canvas;
        }

        private List<Line> DrawCircularBandPassedWave(IReadOnlyList<double> bandPassValues, SolidColorBrush color, double offsetX, double offsetY, double radius)
        {
            var normalizedValues = MathUtils.NormalizeValues(bandPassValues);
            var first = normalizedValues[0];
            var last = normalizedValues[normalizedValues.Count - 1];
            normalizedValues.AddRange(Interpolate(last, first, InterpolationCount));

            var circleCenter = new Point(offsetX, offsetY);

            var maxFluctuation = RadiusScalingFactor * radius;

            var x1 = circleCenter.X + MathUtils.PolarToCartesianCoordinate(0.0, radius + maxFluctuation * normalizedValues[0]).X;
            var y1 = circleCenter.Y + MathUtils.PolarToCartesianCoordinate(0.0, radius + maxFluctuation * normalizedValues[0]).Y;

            var lines = new List<Line>(normalizedValues.Count);
            
            var angleStep = (Math.PI * 2) / (normalizedValues.Count - 1);
            var angleOffset = 0.0;

            //for (var iAmplitudeValue = 0; iAmplitudeValue <= Math.Min(normalizedValues.Count, 100); iAmplitudeValue++)
            //{
            //    var theta = angleStep * iAmplitudeValue;

            //    var X = MathUtils.PolarToCartesianCoordinate(theta, radius + maxFluctuation * normalizedValues[iAmplitudeValue]).X

            //}

            for (var iAmplitudeValue = 1; iAmplitudeValue <= normalizedValues.Count; iAmplitudeValue++)
            {
                var theta = angleStep * iAmplitudeValue;

                if (iAmplitudeValue != normalizedValues.Count)
                {
                    var line = new Line
                    {
                        X1 = x1,
                        Y1 = y1,
                        X2 = circleCenter.X + MathUtils.PolarToCartesianCoordinate(theta, radius + maxFluctuation * normalizedValues[iAmplitudeValue]).X,
                        Y2 = circleCenter.Y + MathUtils.PolarToCartesianCoordinate(theta, radius + maxFluctuation * normalizedValues[iAmplitudeValue]).Y,
                        StrokeThickness = 3,
                        Stroke = color
                    };

                    lines.Add(line);

                    x1 = line.X2;
                    y1 = line.Y2;
                }
                else
                {
                    var line = new Line
                    {
                        X1 = x1,
                        Y1 = y1,
                        X2 = circleCenter.X + MathUtils.PolarToCartesianCoordinate(theta, radius + maxFluctuation * normalizedValues[0]).X,
                        Y2 = circleCenter.Y + MathUtils.PolarToCartesianCoordinate(theta, radius + maxFluctuation * normalizedValues[0]).Y,
                        StrokeThickness = 3,
                        Stroke = color
                    };

                    lines.Add(line);
                    break;
                }

                
            }

            return lines;
        }

        private List<double> GetWaveColorSignificance(List<BandPassFilteredWave> subBandFilterBank)
        {
            var listOfSummedAmplitudes = new List<double>(subBandFilterBank.Count);
            foreach (var wave in subBandFilterBank)
            {
                listOfSummedAmplitudes.Add(wave.SumOfAbsoluteAmplitudes());
            }

            var normalizedListOfSummedAmplitudes = MathUtils.NormalizeValues(listOfSummedAmplitudes);
            return normalizedListOfSummedAmplitudes;
        }

        private void ClearCanvas()
        {
            _canvas.Background = new SolidColorBrush(Color.FromRgb(10, 10, 10));
            _canvas.Children.Clear();
        }

        private List<double> Interpolate(double start, double end, int count)
        {
            List<double> result = new List<double>();
            var step = (end - start) / (count);
            for (var i = 1; i < count - 1; i++)
            {
                result.Add(start + step * i);
            }

            return result;
        }
        public void Dispose()
        {
        }
    }

    public class CircleVisualizer : IDisposable
    {
        private readonly Canvas _canvas;

        private const double RadiusScalingFactor = 0.3;

        public int InterpolationCount = 5;

        public CircleVisualizer(Canvas canvas)
        {
            _canvas = canvas;
        }

        public Canvas Draw(Complex[] fftResult)
        {
            ClearCanvas();

            var centerX = _canvas.ActualWidth / 2;
            var centerY = _canvas.ActualHeight / 2;

            var center = new Point(centerX, centerY);

            var lines = new List<Line>(360);


            for (var i = 0; i < 360; i++)
            {
                var line = new Line
                {
                    X1 = centerX,
                    Y1 = centerY,
                    X2 = fftResult[i].X * 10000 + centerX,
                    Y2 = fftResult[i].Y * 10000 + centerY,
                    StrokeThickness = 3,
                    Stroke = new SolidColorBrush(Color.FromRgb(255, 10, 10))
                };

                lines.Add(line);
            }

            foreach (var line in lines)
            {
                _canvas.Children.Add(line);
            }

            return _canvas;
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
