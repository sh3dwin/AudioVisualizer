﻿using System.Collections.Generic;

namespace AudioVisual.DataStructures
{
    public class BandPassFilteredWave
    {
        public int SampleRate;

        public double MinFrequency;
        public double MaxFrequency;

        public List<double> Values;

        public BandPassFilteredWave(List<double> values, double minFrequency, double maxFrequency, int sampleRate)
        {
            MinFrequency = minFrequency;
            MaxFrequency = maxFrequency;
            Values = values;
            SampleRate = sampleRate;
        }

        public double GetAveragedFrequency()
        {
            return (MaxFrequency + MinFrequency) * 0.5;
        }
    }
}