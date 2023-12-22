﻿using AudioVisual.DataStructures;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using AudioVisual.Analysis;
using AudioVisual.Utils;

namespace AudioVisual.Visualizer
{
    public class CircularFilterBankVisualizer : IDisposable
    {
        private readonly Canvas _canvas;

        private const double RadiusScalingFactor = 0.3;

        public int InterpolationCount = 5;

        public CircularFilterBankVisualizer(Canvas canvas)
        {
            _canvas = canvas;
        }

        public void Dispose()
        {
        }

        public Canvas Draw(List<FrequencyFilter> subBandFilterBank)
        {
            ClearCanvas();

            if (subBandFilterBank[0] is null)
            {
                return _canvas;
            }

            var bandPassCount = subBandFilterBank.Count;
            var canvasLines = new List<Line>((Constants.SegmentCount + InterpolationCount) * bandPassCount);

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
                    // Wave information
                    var filter = subBandFilterBank[iBandPass];
                    var wave = FourierTransformAnalyzer.ToWave(filter, Constants.PowerOfTwo);

                    // Color information
                    var colorValue = Math.Max(colorValues[iBandPass], 0.3);
                    var hue = filter.GetAveragedFrequency() * (360.0 / (Constants.SampleRate / 2.0));
                    var color = FrequencyToColorMapper.ColorFromHSV(hue, 1, colorValue);
                    var brush = new SolidColorBrush(color);

                    // Geometry information
                    var offsetX = columnDistanceBetweenCircles * (column + 1);
                    var radius = Math.Min(rowDistanceBetweenCircles, columnDistanceBetweenCircles) * RadiusScalingFactor;

                    var visualizedWave = DrawCircularWave(wave, brush, offsetX, offsetY, radius);
                    canvasLines.AddRange(visualizedWave);

                    iBandPass++;
                }
            }

            foreach (var line in canvasLines)
            {
                _canvas.Children.Add(line);
            }

            return _canvas;
        }

        private List<Line> DrawCircularWave(IReadOnlyList<float> wave, SolidColorBrush color, double offsetX, double offsetY, double radius)
        {
            var normalizedValues = MathUtils.NormalizeValues(wave);
            var first = normalizedValues[0];
            var last = normalizedValues[normalizedValues.Count - 1];
            normalizedValues.AddRange(Interpolate(last, first, InterpolationCount));

            var lines = new List<Line>((Constants.SegmentCount + InterpolationCount));

            var maxFluctuation = RadiusScalingFactor * radius;

            var circleCenter = new Point(offsetX, offsetY);

            var x1 =
                circleCenter.X + MathUtils.PolarToCartesianCoordinate(0.0, radius + maxFluctuation * normalizedValues[0]).X;
            var y1 =
                circleCenter.Y + MathUtils.PolarToCartesianCoordinate(0.0, radius + maxFluctuation * normalizedValues[0]).Y;

            var step = wave.Count / (Constants.SegmentCount + InterpolationCount);
            var angleStep = (Math.PI * 2) / (Constants.SegmentCount + InterpolationCount);

            for (var iSegment = 1; iSegment <= (Constants.SegmentCount + InterpolationCount); iSegment++)
            {
                var theta = angleStep * iSegment;

                if (iSegment != (Constants.SegmentCount + InterpolationCount))
                {
                    var fluctuation = maxFluctuation * normalizedValues[iSegment * step];
                    var line = new Line
                    {
                        X1 = x1,
                        Y1 = y1,
                        X2 = circleCenter.X + MathUtils.PolarToCartesianCoordinate(theta, radius + fluctuation).X,
                        Y2 = circleCenter.Y + MathUtils.PolarToCartesianCoordinate(theta, radius + fluctuation).Y,
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

        private List<double> GetWaveColorSignificance(IReadOnlyList<FrequencyFilter> subBandFilterBank)
        {
            var listOfSummedAmplitudes = new List<float>(subBandFilterBank.Count);
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
    }
}