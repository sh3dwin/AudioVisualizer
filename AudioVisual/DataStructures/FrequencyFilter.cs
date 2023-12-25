using System;
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
            BinWidth = (double)Constants.SampleRate / fftValues.Count;

            var lowerBin = (int)(lowerFrequencyBoundary / BinWidth);
            var upperBin = (int)(upperFrequencyBoundary / BinWidth);

            var filteredFrequencies = new List<FftFrequencyBin>(upperBin - lowerBin);

            for (var i = 0; i < fftValues.Count; i++)
            {
                if (fftValues[i].Frequency > lowerFrequencyBoundary
                    &&
                    fftValues[i].Frequency < upperFrequencyBoundary)
                {
                    filteredFrequencies.Add(fftValues[i]);
                }
            }

            Values = filteredFrequencies;
            MaxFrequency = Values[(Values.Count / 2) - 1].Frequency;
            MinFrequency = Values[0].Frequency;
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

            return result.ToArray();
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
            {
                return Values.Sum(x => Math.Abs(x.Amplitude));
            }

            var tempList = Values;
            tempList.Sort((x, y) => (int)(x.Amplitude - y.Amplitude));

            return tempList.GetRange(0, 5).Sum(x => Math.Abs(x.Amplitude));
        }
    }
}
