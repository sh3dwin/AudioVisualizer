using System;
using System.Collections.Generic;
using System.Windows;
using NAudio.Dsp;

namespace AudioVisual.Utils
{
    public static class MathUtils
    {
        public static List<int> SplitIntoNGeometricSeries(int n, int size)
        {
            var step = 0;
            var sizeOfSmallestSplit = 100;
            if (n * sizeOfSmallestSplit > size / 2)
            {
                sizeOfSmallestSplit = (int)((size * 0.5) / n);
            }
            var coefficient = GetGeometricSeriesCoefficient(n, size - (sizeOfSmallestSplit * n));
            var splits = new List<int>();
            while (step < n)
            {
                var value = (int)Math.Floor(Math.Pow(coefficient, step + 1));
                splits.Add(value + sizeOfSmallestSplit);
                step++;
            }
            return splits;
        }

        public static double GetGeometricSeriesCoefficient(int n, int size)
        {
            // Get larger than boundary
            var largerThan = Math.Exp(Math.Log(size - (n + 1)) / (n + 1));
            // Get smaller than boundary
            var smallerThan = Math.Exp(Math.Log(size - n) / n);

            return BinarySearchCoefficient(largerThan, smallerThan, n, size);
        }

        public static double BinarySearchCoefficient(double largerThan, double smallerThan, int n, int size)
        {
            while (true)
            {
                var coefficient = (smallerThan + largerThan) * 0.5;
                var sum = GeometricSeriesSum(coefficient, n);
                if (Math.Abs(size - sum) < 5) return coefficient;

                if (sum > size)
                {
                    var lowerSum = GeometricSeriesSum(largerThan, n);
                    if (lowerSum > size) largerThan -= 0.001;
                    smallerThan = coefficient;
                }
                else
                {
                    largerThan = coefficient;
                }
            }
        }

        public static double GeometricSeriesSum(double coefficient, int n)
        {
            return (Math.Pow(coefficient, n + 1) - 1) / (coefficient - 1);
        }

        public static Point PolarToCartesian(this Point point)
        {
            var x = point.X * Math.Cos(point.Y);
            var y = point.X * Math.Sin(point.Y);
            var result = new Point(x, y);
            return result;
        }
    }
}
