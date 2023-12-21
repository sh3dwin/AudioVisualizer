using System;
using System.Collections.Generic;
using AudioVisual.Utils;
using NAudio.Dsp;

namespace AudioVisual.Processor
{
    public class FrequencySpectrumAggregator : IProcessor
    {
        private List<int> _frequencyPartitionsSplits;

        private int _samplesNumber;
        private int _sampleRate;
        private int _maxFrequency;

        private Complex[] _frequencyValues;

        public FrequencySpectrumAggregator(int samplesNumber = 200, int sampleRate = 48000)
        {
            _samplesNumber = samplesNumber;
            _sampleRate = sampleRate;
            _maxFrequency = (int)(sampleRate * 0.5);
        }

        public int Partitions
        {
            get => _samplesNumber;
            set {
            _samplesNumber = value;
            _frequencyPartitionsSplits = MathUtils.SplitIntoNGeometricSeries(_samplesNumber, (int)(_frequencyValues.Length * 0.25));
            }
        }
        public int SampleRate
        {
            get => _sampleRate;
            set
            {
                _sampleRate = value;
                _maxFrequency = (int)(_sampleRate * 0.5);
            }
        }
        public List<double> GetHues()
        {
            var frequencyFraction = 0.25;
            var frequencyStep = (_maxFrequency * frequencyFraction) / Partitions;
            var hueStep = 720.0 / (_maxFrequency * frequencyFraction);
            var splitLowerBound = 0;
            
            var hues = new List<double>(Partitions);
            for (int i = 0; i < Partitions; i++)
            {
                var splitUpperBound = (splitLowerBound + _frequencyPartitionsSplits[i]) * frequencyStep;
                var meanFrequency = (splitUpperBound + splitLowerBound ) * 0.5;
                var hue = hueStep * meanFrequency;

                splitLowerBound += _frequencyPartitionsSplits[i];

                hues.Add(hue);
            }

            return hues;
        }
        public List<double> GetAggregatedFrequencies(Complex[] frequencySpectrum)
        {
            _frequencyValues = frequencySpectrum;

            _frequencyPartitionsSplits ??= MathUtils.SplitIntoNGeometricSeries(Partitions, (int)(frequencySpectrum.Length * 0.25));

            var splitLowerBound = 0;
            var summedFrequencies = new List<double>(Partitions);

            for (int splitIndex = 0; splitIndex < Partitions; splitIndex++)
            {
                var summedValues = 
                    GetMaxAmplitudeBetweenFrequencies(splitLowerBound, splitLowerBound + _frequencyPartitionsSplits[splitIndex]) * Math.Log(splitIndex + 1);
                splitLowerBound += _frequencyPartitionsSplits[splitIndex];

                summedFrequencies.Add(summedValues);
            }
            return summedFrequencies;
        }
        private double GetMaxAmplitudeBetweenFrequencies(double lowerFrequencyBoundary, double upperFrequencyBoundary)
        {
            var maxAmplitude = 0.0;
            for (int i = (int)lowerFrequencyBoundary; i < upperFrequencyBoundary && i < _frequencyValues.Length; i++)
            {
                if (_frequencyValues[i].X > maxAmplitude)
                    maxAmplitude = _frequencyValues[i].X;
            }

            return maxAmplitude;
        }
    }
}
