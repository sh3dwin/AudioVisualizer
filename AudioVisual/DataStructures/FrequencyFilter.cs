﻿using System;
using System.Collections.Generic;
using System.Linq;
using AudioVisual.Utils;
using NAudio.Dsp;

namespace AudioVisual.DataStructures
{
    public class FrequencyFilter
    {
        public double MinFrequency;
        public double MaxFrequency;

        public double BinWidth;

        public List<FftFrequencyBin> Values;

        public FrequencyFilter(List<FftFrequencyBin> values)
        {
            Values = values;
        }

        public FrequencyFilter(IReadOnlyList<FftFrequencyBin> fftValues, double lowerFrequencyBoundary, double upperFrequencyBoundary)
        {
            // TODO: check whether the Nyquist frequency has been reached and set the frequency of the affected bins accordingly
            const int maxFrequency = Constants.SampleRate;
            BinWidth = (double)maxFrequency / fftValues.Count;
            var minFrequencyBin = (int)(lowerFrequencyBoundary / BinWidth);
            var maxFrequencyBin = (int)(upperFrequencyBoundary / BinWidth);

            var filteredFrequencies = new List<FftFrequencyBin>(maxFrequencyBin - minFrequencyBin);

            for (var i = minFrequencyBin; i < maxFrequencyBin; i++)
            {
                if (i >= fftValues.Count)
                    break;
                filteredFrequencies.Add(fftValues[i]);
            }

            // Make sure the frequencies are in ascending order
            filteredFrequencies.Sort((x, y) => (int)(x.Frequency - y.Frequency));

            Values = filteredFrequencies;
            MinFrequency = Values[0].Frequency;
            MaxFrequency = Values[Values.Count - 1].Frequency;
        }

        public Complex[] ToPaddedComplexArray(int m)
        {
            var count = (int)Math.Pow(2, m);
            var zeroComplex = new Complex()
            {
                X = 0,
                Y = 0
            };

            var result = Enumerable.Repeat(zeroComplex, count).ToArray();

            var iValues = 0;

            for (var iResult = 0; iResult < count; iResult++)
            {
                var binValue = iResult * BinWidth;
                if (binValue < MinFrequency || binValue > MaxFrequency)
                    continue;

                result[iResult] = Values[iValues].ToComplex();

                iValues++;
            }

            return result;
        }

        public double GetAveragedFrequency()
        {
            return (MaxFrequency + MinFrequency) * 0.5;
        }

        public float SumOfAbsoluteAmplitudes()
        {
            return Values.Sum(x => Math.Abs(x.Amplitude));
        }

        public float SumOfFiveBiggestContributingFrequencies()
        {
            if (Values.Count < 5)
                throw new Exception("Frequency filter contains less than 5 frequency bins!");

            var tempList = Values;
            tempList.Sort((x, y) => (int)(x.Amplitude - y.Amplitude));

            return tempList.GetRange(0, 5).Sum(x => Math.Abs(x.Amplitude));
        }
    }
}
