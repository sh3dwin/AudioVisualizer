using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace AudioVisual.Utils
{
    public static class MathUtils
    {
        public static List<double> SinValues0ToPiHalf;
        public const double HalfPi = Math.PI / 2;
        public static List<int> SplitIntoNGeometricSeries(int n, int size)
        {
            int step = 0;
            int sizeOfSmallestSplit = 100;
            if (n * sizeOfSmallestSplit > size / 2)
            {
                sizeOfSmallestSplit = (int)((size * 0.5) / n);
            }
            double coefficient = GetGeometricSeriesCoefficient(n, size - (sizeOfSmallestSplit * n));
            List<int> splits = new List<int>();
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
            double largerThan = Math.Exp(Math.Log(size - (n + 1)) / (n + 1));
            // Get smaller than boundary
            double smallerThan = Math.Exp(Math.Log(size - n) / n);

            return BinarySearchCoefficient(largerThan, smallerThan, n, size);
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

        public static Point PolarToCartesianCoordinate(double theta, double radius)
        {
            var x = radius * Math.Cos(theta);
            var y = radius * Math.Sin(theta);
            var result = new Point(x, y);
            return result;
        }

        public static List<double> NormalizeValues(IReadOnlyList<double> values)
        {
            var maxAmplitude = values.Max(Math.Abs);
            const double epsilon = 1e-3;
            if (maxAmplitude < epsilon)
                return values.Select(_ => 0.0).ToList();
            var normalizationFactor = 1.0 / maxAmplitude;
            return values.Select(x => x * normalizationFactor).ToList();
        }

        public static double FastSin(double value)
        {
            value %= Math.PI * 2.0;
            var sign = (value > Math.PI) ? -1.0 : 1.0;
            var left = ((value % Math.PI) < HalfPi);
            value %= (HalfPi);
            var sinusValue = GetSinValueAt(value, left);

            return sign * sinusValue;
        }

        public static double GetSinValueAt(double x, bool left)
        {
            if (SinValues0ToPiHalf is null)
                throw new Exception("The values of the sinus function has not been evaluated");
            // Normalize x between 0 and 1
            x /= HalfPi;
            // 21600 = 60 * 360
            var index = (int)Math.Floor(x * (SinValues0ToPiHalf.Count - 1));
            if (!left)
                index = (SinValues0ToPiHalf.Count - 1) - index;
            return SinValues0ToPiHalf[index];
        }

        public static double Sin(double value)
        {
            const double b = 4.0 / Math.PI;
            const double c = -4 / (Math.PI * Math.PI);

            return -(b * value + c * value * ((value < 0) ? -value : value));
        }

        public static void InitializeSinValuesBetween0AndPi()
        {
            var sinValues = new List<double>(21600);
            var step = HalfPi / 21600;
            for (var x = 0.0; x < HalfPi; x += step)
            {
                sinValues.Add(MathUtils.Sin(x));
            }
            SinValues0ToPiHalf = sinValues;
        }

    }
}
