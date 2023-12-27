using System.Collections.Generic;
using AudioVisual.DataStructures;
using AudioVisual.Utils;

namespace AudioVisual.Processor
{
    public class FilterBank : IProcessor
    {
        private List<int> _bandPassSplits;
        private int _bandPassCount;

        private List<int> _frequencyPartitionsSplits;

        private List<FftFrequencyBin> _fftValues;
        
        public FilterBank(int bandPassCount = 3)
        {
            _bandPassCount = bandPassCount;
            
            var maxFrequency = (int)(Globals.SampleRate * Constants.FrequencyFraction);
            _bandPassSplits = MathUtils.SplitIntoNGeometricSeries(_bandPassCount, maxFrequency);
        }
        public int BandPassCount
        {
            get => _bandPassCount;
            set
            {
                _bandPassCount = value;
                var maxFrequency = (int)(Globals.SampleRate * Constants.FrequencyFraction);
                _bandPassSplits =
                    MathUtils.SplitIntoNGeometricSeries(_bandPassCount, maxFrequency);
            }
        }

        public List<FrequencyFilter> GetFrequencyFilters(List<FftFrequencyBin> fftResult, int wavePartitionsCount)
        {
            _fftValues = fftResult;
            BandPassCount = wavePartitionsCount;

            var splitLowerBound = 0;
            var allFrequencyWindows = new List<FrequencyFilter>(_bandPassCount + 1);

            // If the number of partitions is only one, return only the whole wave
            if (BandPassCount == 1)
            {
                return new List<FrequencyFilter>
                {
                    new(_fftValues, 0, (Globals.SampleRate * Constants.FrequencyFraction))
                };
            }

            for (var iSplit = 0; iSplit < _bandPassCount; iSplit++)
            {
                var lowerFrequencyBoundary = splitLowerBound;
                var upperFrequencyBoundary = splitLowerBound + _bandPassSplits[iSplit];
                var frequencyFilter = 
                    new FrequencyFilter(_fftValues, lowerFrequencyBoundary, upperFrequencyBoundary);

                splitLowerBound += _bandPassSplits[iSplit];

                allFrequencyWindows.Add(frequencyFilter);
            }

            var wholeWave = 
                new FrequencyFilter(_fftValues, 1, (Globals.SampleRate * Constants.FrequencyFraction));
            allFrequencyWindows.Add(wholeWave);

            return allFrequencyWindows;
        }

        public List<double> GetAggregatedFrequencies(List<FftFrequencyBin> fftResult)
        {
            _fftValues?.Clear();
            _frequencyPartitionsSplits?.Clear();

            _fftValues = fftResult;

            var size = (int)(Globals.SampleRate * Constants.FrequencyFraction);
            _frequencyPartitionsSplits = MathUtils.SplitIntoNGeometricSeries(Constants.SegmentCount, size);

            var splitLowerBound = 1;
            var summedFrequencies = new List<double>(Constants.SegmentCount);

            for (var iSplit = 0; iSplit < Constants.SegmentCount; iSplit++)
            {
                var lowerFrequencyBoundary = splitLowerBound;
                var upperFrequencyBoundary = splitLowerBound + _frequencyPartitionsSplits[iSplit];
                var frequencyFilter =
                    new FrequencyFilter(_fftValues, lowerFrequencyBoundary, upperFrequencyBoundary);

                var sumOfSignificantAmplitudes = frequencyFilter.SumOfSignificantAmplitudes();

                splitLowerBound += _frequencyPartitionsSplits[iSplit];

                summedFrequencies.Add(sumOfSignificantAmplitudes);
            }

            return summedFrequencies;
        }
    }
}
