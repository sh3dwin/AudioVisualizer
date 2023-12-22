using System;
using System.Collections.Generic;
using AudioVisual.DataStructures;
using AudioVisual.Utils;

namespace AudioVisual.Processor
{
    public class FrequencySpectrumAggregator : IProcessor
    {
        private List<int> _frequencyPartitionsSplits;

        private int _samplesNumber;
        private int _sampleRate;
        private int _maxFrequency;

        private List<FftFrequencyBin> _fftValues;

        public FrequencySpectrumAggregator(int samplesNumber = Constants.SegmentCount, int sampleRate = Constants.SampleRate)
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
            _frequencyPartitionsSplits = MathUtils.SplitIntoNGeometricSeries(_samplesNumber, (int)(_fftValues.Count));
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
            var frequencyStep = (_maxFrequency) / Partitions;
            var hueStep = 720.0 / (_maxFrequency);
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
        public List<double> GetAggregatedFrequencies(List<FftFrequencyBin> fftResult)
        {
            _fftValues = fftResult;

            _frequencyPartitionsSplits ??= MathUtils.SplitIntoNGeometricSeries(Partitions, (int)(fftResult.Count));

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
            for (int i = (int)lowerFrequencyBoundary; i < upperFrequencyBoundary && i < _fftValues.Count; i++)
            {
                if (_fftValues[i].Amplitude > maxAmplitude)
                    maxAmplitude = _fftValues[i].Amplitude;
            }

            return maxAmplitude;
        }
    }
}
