﻿using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Shapes;
using NAudio.Dsp;
using NAudio.Wave;

namespace AudioVisual
{
    public static class SoundWaveUtils
    {
        public static Complex[] CreateAndInitializeComplexArray(WaveBuffer buffer, int M)
        {
            int len = buffer.FloatBuffer.Length / 8;

            Complex[] values = new Complex[(int)Math.Pow(2, M) +len];
            for (int i = 0; i < len; i++)
            {
                values[i].Y = 0;
                values[i].X = buffer.FloatBuffer[i];
            }

            return values;
        }

        public static double SumOfLowFrequencyAmplitudes(List<List<Complex>> aggregatedList, int numSplits)
        {
            double sum = 0;
            for (int i = 0; i < numSplits / 3; i++)
            {
                foreach (var item in aggregatedList[i])
                {
                    sum += MathUtils.Magnitude(item);
                }
            }
            return sum;
        }

        public static double SumOfMidFrequencyAmplitudes(List<List<Complex>> aggregatedList, int numSplits)
        {
            double sum = 0;
            for (int i = numSplits / 3; i < numSplits * 2 / 3; i++)
            {
                foreach (var item in aggregatedList[i])
                {
                    sum += MathUtils.Magnitude(item);
                }
            }
            return sum;
        }
        public static double SumOfHighFrequencyAmplitudes(List<List<Complex>> aggregatedList, int numSplits)
        {
            double sum = 0;
            for (int i = numSplits * 2 / 3; i < numSplits; i++)
            {
                foreach (var item in aggregatedList[i])
                {
                    sum += MathUtils.Magnitude(item);
                }
            }
            return sum;
        }
        public static List<List<Complex>> Aggregate(NAudio.Dsp.Complex[] values, int nSplits, int count)
        {
            // Get splits
            var splits = MathUtils.SplitIntoNGeometricSeries(nSplits, count);

            var aggregatedValues = new List<List<Complex>>();

            // Initialize list
            for (int i = 0; i < nSplits; i++)
            {
                aggregatedValues.Add(new List<Complex>());
            }

            return FillAggregatedList(aggregatedValues, splits, values, count);
        }

        public static List<List<Complex>> FillAggregatedList(List<List<Complex>> aggregatedList, List<int> splits, Complex[] values, int count)
        {
            aggregatedList.Clear();
            for (int i = 0; i < splits.Count; i++)
            {
                aggregatedList.Add(new List<Complex>());
            }
            for (int i = 0; i < count; i++)
            {
                aggregatedList[GetSmallestSplitLargerThanIndex(i, splits)].Add(new Complex { X = (float)(values[i].X * Math.Log(i + 2)), Y = values[i].Y });
            }

            return aggregatedList;
        }

        public static int GetSmallestSplitLargerThanIndex(double value, List<int> splits)
        {
            int index = 0;
            int sum = splits[0];
            while (value > sum && index != splits.Count - 1)
            {
                index++;
                sum += splits[index];
            }
            return index;
        }

        public static double GetTopNOrTop5Average(List<Complex> values)
        {
            if (values.Count == 0)
                return 0.0;
            var count = 5;
            if (values.Count < 5)
                count = values.Count;
            values.Sort((x, y) => y.X.CompareTo(x.X));
            var top5 = values.GetRange(0, count);
            double top5Sum = 0;
            foreach (var val in top5)
            {
                top5Sum += Math.Abs(val.X);
            }
            return top5Sum / count;
            //return MathUtils.Magnitude(values[0]);
        }
        public static Rectangle CreateRectangle(double height, double width, SolidColorBrush brush)
        {
            var rect = new Rectangle();
            rect.Fill = brush;
            rect.Height = height;
            rect.Width = width;

            return rect;
        }
    }
}
