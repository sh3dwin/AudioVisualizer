using System;
using System.Collections.Generic;

namespace AudioVisual.Utils
{
    public static class MathUtils
    {
        public static List<int> SplitIntoNGeometricSeries(int n, int size)
        {
            int step = 0;
            double coefficient = GetGeometricSeriesCoefficient(n, size);
            List<int> splits = new List<int>();
            while (step < n)
            {
                var value = (int)Math.Floor(Math.Pow(coefficient, step + 1));
                splits.Add(value);
                step++;
            }
            return splits;
        }

        public static double GetGeometricSeriesCoefficient(int n, int size)
        {
            // Get larger than boundary
            double largerThan = Math.Exp(Math.Log(size - (n + 1)) / (n + 1));
            // Get smaller than boundary
            double smallerThan = Math.Exp(Math.Log(size - n) / n);

            return BinarySearchCoefficient(largerThan, smallerThan, n, size);
        }
        public static double FastSin(double x)
        {
            const double B = 4.0 / Math.PI;
            const double C = -4 / (Math.PI * Math.PI);

            return -(B * x + C * x * ((x < 0) ? -x : x));
        }

        public static double BinarySearchCoefficient(double largerThan, double smallerThan, int n, int size)
        {
            double coefficient = (smallerThan + largerThan) * 0.5;
            double sum = GeometricSeriesSum(coefficient, n);
            if (Math.Abs(size - sum) < 5)
                return coefficient;

            if (sum > size)
            {
                double lowerSum = GeometricSeriesSum(largerThan, n);
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

        public static double Magnitude(NAudio.Dsp.Complex complex)
        {
            return new System.Numerics.Complex(complex.X, complex.Y).Magnitude;
        }

        public static double Argument(NAudio.Dsp.Complex complex)
        {
            return new System.Numerics.Complex(complex.X, complex.Y).Phase;
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

    }
}
