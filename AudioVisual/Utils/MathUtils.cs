using System;
using System.Collections.Generic;
using System.Windows;

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
            var coefficient = (smallerThan + largerThan) * 0.5;
            var sum = GeometricSeriesSum(coefficient, n);
            if (Math.Abs(size - sum) < 5)
                return coefficient;

            if (sum > size)
            {
                var lowerSum = GeometricSeriesSum(largerThan, n);
                if (lowerSum > size)
                    largerThan -= 0.001;
                return BinarySearchCoefficient(largerThan, coefficient, n, size);
            }
            else
                return BinarySearchCoefficient(coefficient, smallerThan, n, size);
        }

        public static double GeometricSeriesSum(double coefficient, int n)
        {
            return (Math.Pow(coefficient, n + 1) - 1) / (coefficient - 1);
        }

        public static double AddLinkedListElements(LinkedList<double> list)
        {
            var firstWeight = 0.4;
            var weight = (1 - firstWeight) / (list.Count - 1);
            
            double sum = 0;
            var current = list.First;
            sum += current.Value * firstWeight;
            current = current.Next;
            while (current != null)
            {
                sum += current.Value * weight;
                current = current.Next;
            }
            return sum;
        }

        public static Point PolarToCartesianCoordinate(double theta, double radius)
        {
            var x = radius * Math.Cos(theta);
            var y = radius * Math.Sin(theta);
            var result = new Point(x, y);
            return result;
        }
    }
}
