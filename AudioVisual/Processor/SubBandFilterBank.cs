using System;
using System.Collections.Generic;
using System.Linq;
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

        private List<FftFrequencyBin> _fftValues;
        private List<double> _wholeWaveAmplitudes;

        private double _samplingPeriod;
        private double _binWidth;
        public SubBandFilterBank(int samplesCount = 200, int bandPassCount = 3, int sampleRate = 48000)
        {
            _bandPassCount = bandPassCount;
            _samplesCount = samplesCount;
            _sampleRate = sampleRate;

            var maxFrequency = (int)(_sampleRate * 0.5);
            _bandPassSplits = MathUtils.SplitIntoNGeometricSeries(_bandPassCount, maxFrequency);

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
                    MathUtils.SplitIntoNGeometricSeries(_bandPassCount, maxFrequency);
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

        public List<FrequencyFilter> GetSubBandFilterBank(List<FftFrequencyBin> fftResult)
        {
            _fftValues = fftResult;

            var binCount = _fftValues.Count;
            // frequency change between bins
            _binWidth = (double)_sampleRate / (binCount * 2);

            _wholeWaveAmplitudes = new List<double>();

            var splitLowerBound = 0;
            var allFrequencyWindows = new List<FrequencyFilter>(_bandPassCount + 1);

            // If the number of partitions is only one, return only the whole wave
            if (BandPassCount == 1)
            {
                return new List<FrequencyFilter>
                {
                    new(_fftValues, 0, (_sampleRate * 0.5), _sampleRate)
                };
            }

            for (var iSplit = 0; iSplit < _bandPassCount; iSplit++)
            {
                var lowerFrequencyBoundary = splitLowerBound;
                var upperFrequencyBoundary = splitLowerBound + _bandPassSplits[iSplit];
                var frequencyFilter = new FrequencyFilter(_fftValues, lowerFrequencyBoundary, upperFrequencyBoundary, _sampleRate);

                splitLowerBound += _bandPassSplits[iSplit];

                allFrequencyWindows.Add(frequencyFilter);
            }

            var wholeWave = new FrequencyFilter(_fftValues, 0, (_sampleRate * 0.5), _sampleRate);
            allFrequencyWindows.Add(wholeWave);

            return allFrequencyWindows;
        }

        //private double GetAmplitude(double lowerFrequencyBoundary, double upperFrequencyBoundary, int t)
        //{
        //    var minFrequencyBin = lowerFrequencyBoundary / _binWidth;
        //    var maxFrequencyBin = upperFrequencyBoundary / _binWidth;
        //    var sumOfAmplitudes = 0.0;
        //    var bufferSize = 4756;
        //    const double epsilon = 1e-3;
        //    for (var i = (int)minFrequencyBin; i < (int)maxFrequencyBin; i++)
        //    {
        //        if (_fftValues[i].Amplitude < epsilon && _fftValues[i].Amplitude > -epsilon)
        //            continue;

        //        var arg = Math.PI * 2.0 * _fftValues[i].Frequency * t; 
        //        sumOfAmplitudes += _fftValues[i].Amplitude * MathUtils.FastSin(arg + (_fftValues[i].PhaseShift + Math.PI * 2));
        //    }
        //    if (_wholeWaveAmplitudes.Count > t)
        //        _wholeWaveAmplitudes[t] += sumOfAmplitudes;
        //    else
        //        _wholeWaveAmplitudes.Add(sumOfAmplitudes);

        //    return sumOfAmplitudes;
        //}
    }
}
