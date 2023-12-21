﻿using System;
using System.Collections.Generic;
using AudioVisual.DataStructures;
using AudioVisual.Utils;
using NAudio.Dsp;

namespace AudioVisual.Processor
{
    public class SubBandFilterBank : IProcessor
    {
        private List<int> _bandPassSplits;
        private int _bandPassCount;

        private int _samplesCount;
        private int _sampleRate;

        private Complex[] _frequencyValues;
        private List<double> _wholeWaveAmplitudes;

        private double _samplingPeriod;
        private double _binWidth;
        public SubBandFilterBank(int samplesCount = 200, int bandPassCount = 3, int sampleRate = 48000)
        {
            _bandPassCount = bandPassCount;
            _samplesCount = samplesCount;
            _sampleRate = sampleRate;

            var maxFrequency = (int)(_sampleRate * 0.5);
            _bandPassSplits = MathUtils.SplitIntoNGeometricSeries(_bandPassCount, maxFrequency / 4);

            _samplingPeriod = 1.0 / _sampleRate;

        }
        public int BandPassCount
        {
            get => _bandPassCount;
            set
            {
                _bandPassCount = value;
                var maxFrequency = (int)(_sampleRate * 0.5);
                _bandPassSplits =
                    MathUtils.SplitIntoNGeometricSeries(_bandPassCount, (maxFrequency / 4));
            }

        }

        public int SamplesCount
        {
            get => _samplesCount;
            set => _samplesCount = value;
        }

        public int SampleRate
        {
            get => _sampleRate;
            set
            {
                _sampleRate = value;
                _samplingPeriod = 1.0 / _sampleRate;
            }
        }

        public List<BandPassFilteredWave> GetSubBandFilterBank(Complex[] frequencySpectrum)
        {
            _frequencyValues = frequencySpectrum;

            var binCount = _frequencyValues.Length;
            // frequency change between bins
            _binWidth = (double)_sampleRate / (binCount * 2);

            _wholeWaveAmplitudes = new List<double>();

            var splitLowerBound = 0;
            var allFrequencyWindows = new List<BandPassFilteredWave>(_bandPassCount + 1);

            // If the number of partitions is only one, return only the whole wave
            if (BandPassCount == 1)
            {
                return new List<BandPassFilteredWave> { GetBandPassedWave(0, _bandPassSplits[0]) };
            }

            for (var iSplit = 0; iSplit < _bandPassCount; iSplit++)
            {

                var bandPassFilteredWave = GetBandPassedWave(splitLowerBound, splitLowerBound + _bandPassSplits[iSplit]);

                splitLowerBound += _bandPassSplits[iSplit];

                allFrequencyWindows.Add(bandPassFilteredWave);
            }

            var wholeWave =
                new BandPassFilteredWave(_wholeWaveAmplitudes, 0, _sampleRate * 0.125, _sampleRate);
            allFrequencyWindows.Add(wholeWave);

            return allFrequencyWindows;
        }

        private BandPassFilteredWave GetBandPassedWave(double lowerFrequencyBoundary, double upperFrequencyBoundary)
        {
            var values = new List<double>(_samplesCount);
            for (var i = 0; i < _samplesCount; ++i)
            {
                var value = GetAmplitude(lowerFrequencyBoundary, upperFrequencyBoundary, i);
                values.Add(value);
            }

            return new BandPassFilteredWave(values, lowerFrequencyBoundary, upperFrequencyBoundary, _sampleRate);
        }

        private double GetAmplitude(double lowerFrequencyBoundary, double upperFrequencyBoundary, int index)
        {
            var frequency = lowerFrequencyBoundary;
            var minFrequencyBin = lowerFrequencyBoundary / _binWidth;
            var maxFrequencyBin = upperFrequencyBoundary / _binWidth;
            var sumOfAmplitudes = 0.0;
            var bufferSize = 4756;
            var stepSize = (Math.PI * 2.0) / _samplesCount;
            const double epsilon = 1e-3;
            for (var i = (int)minFrequencyBin; i < maxFrequencyBin && i < _samplesCount; i++)
            {
                if (_frequencyValues[i].X < epsilon && _frequencyValues[i].X > -epsilon)
                    continue;

                var arg = (index * frequency) * stepSize; 
                sumOfAmplitudes += _frequencyValues[i].X * MathUtils.FastSin(arg + MathUtils.HalfPi);
                frequency += _binWidth;
            }
            if (_wholeWaveAmplitudes.Count > index)
                _wholeWaveAmplitudes[index] += sumOfAmplitudes;
            else
                _wholeWaveAmplitudes.Add(sumOfAmplitudes);

            return sumOfAmplitudes;
        }
    }
}
