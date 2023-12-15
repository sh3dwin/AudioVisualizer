using System;
using System.Collections.Generic;
using System.Linq;
using NAudio.Dsp;

namespace AudioVisual
{
    public class FrequencyIntoWaveAggregator : IProcessor
    {
        private List<int> _wavePartitionsSplits;
        private int _wavePartitionsNumber;

        private int _samplesNumber;
        private int _sampleRate;

        private Complex[] _frequencyValues;
        private List<double> _finalWaveAmplitudeValues;

        private double _denom;
        private double _binSize;

        private double _maxSum = 0;
        public FrequencyIntoWaveAggregator(int samplesNumber = 200, int wavePartitionNumber = 3, int sampleRate = 48000)
        {
            _wavePartitionsNumber = wavePartitionNumber;
            _samplesNumber = samplesNumber;
            _sampleRate = sampleRate;

            var maxFrequency = (int)(_sampleRate * 0.5);
            _wavePartitionsSplits = MathUtils.SplitIntoNGeometricSeries(_wavePartitionsNumber, maxFrequency);

            _denom = 1.0 / _sampleRate;

        }
        public int WavePartitionNumber
        {
            get => _wavePartitionsNumber;
            set
            {
                _wavePartitionsNumber = value;
                _wavePartitionsSplits =
                    MathUtils.SplitIntoNGeometricSeries(_wavePartitionsNumber, (int)(_sampleRate * 0.5));
            }

        }

        public int SamplesPerWave
        {
            get => _samplesNumber;
            set => _samplesNumber = value;
        }

        public int SampleRate
        {
            get => _sampleRate;
            set
            {
                _sampleRate = value;
                _denom = 1.0 / _sampleRate;
            }
        }
        public List<double> GetHues()
        {
            var hueStep = 720.0 / SampleRate;
            var splitLowerBound = 0;
            var hues = new List<double>(_wavePartitionsNumber + 1);
            for (int i = 0; i < _wavePartitionsNumber; i++)
            {
                var splitUpperBount = splitLowerBound + _wavePartitionsSplits[i];
                var averageFrequency = (splitUpperBount + splitLowerBound) * 0.5;
                var hue = hueStep * averageFrequency;

                splitLowerBound += _wavePartitionsSplits[i];

                hues.Add(hue);
            }
            hues.Add(_sampleRate * 0.25);

            return hues;
        }
        public List<List<double>> GetSummedWaves(Complex[] frequencySpectrum)
        {
            _frequencyValues = frequencySpectrum;

            var binCount = _frequencyValues.Length;
            // frequency change from bin to bin
            _binSize = (double)_sampleRate / (binCount * 2);

            _finalWaveAmplitudeValues = new List<double>();

            var splitLowerBound = 0;
            var summedWaves = new List<List<double>>(_wavePartitionsNumber + 1);
            for (int splitIndex = 0; splitIndex < _wavePartitionsNumber; splitIndex++)
            {
                var summedValues = GetSummedWaveValues(splitLowerBound, splitLowerBound + _wavePartitionsSplits[splitIndex]);
                splitLowerBound += _wavePartitionsSplits[splitIndex];

                summedWaves.Add(summedValues);
            }
            summedWaves.Add(_finalWaveAmplitudeValues);
            //return NormalizeValues(summedWaves);
            return summedWaves;
        }
        private List<double> GetSummedWaveValues(double lowerFrequencyBoundary, double upperFrequencyBoundary)
        {
            var values = new List<double>(_samplesNumber);

            for (int i = 0; i < _samplesNumber; ++i)
            {
                var sum = SumAllAmplitudesBetweenFrequencies(lowerFrequencyBoundary, upperFrequencyBoundary, i);
                if (sum > _maxSum)
                    _maxSum = sum;
                values.Add(sum);
            }

            return values;
        }
        private double SumAllAmplitudesBetweenFrequencies(double lowerFrequencyBoundary, double upperFrequencyBoundary, int index)
        {
            double frequency = lowerFrequencyBoundary;
            var minFrequencyBin = lowerFrequencyBoundary / _binSize;
            var maxFrequencyBin = upperFrequencyBoundary / _binSize;
            var sumOfAmplitudes = 0.0;
            var constant = 2 * Math.PI * index;
            for (int i = (int)minFrequencyBin; i < maxFrequencyBin && i < _samplesNumber; i++)
            {
                //Console.WriteLine($"Frequency is {frequency} at bin {i}, min frequency at {minFrequencyBin}, maximum at {maxFrequencyBin}");
                sumOfAmplitudes += _frequencyValues[i].X * Math.Sin((constant * frequency + MathUtils.Argument(_frequencyValues[i])) * _denom);
                frequency += _binSize;
            }
            if (_finalWaveAmplitudeValues.Count > index)
                _finalWaveAmplitudeValues[index] += sumOfAmplitudes;
            else
                _finalWaveAmplitudeValues.Add(sumOfAmplitudes);

            return sumOfAmplitudes;
        }

        private List<List<double>> NormalizeValues(List<List<double>> summedWaves)
        {
            var normalizedValues = new List<List<double>>();
            for (int i = 0; i < summedWaves.Count; i++)
            {
                normalizedValues.Add(new List<double>());
                var scalingFactor = 1 / (_maxSum);
                normalizedValues[i].AddRange(summedWaves[i].Select(x => x * scalingFactor));
            }

            return normalizedValues;
        }

        
    }
}
