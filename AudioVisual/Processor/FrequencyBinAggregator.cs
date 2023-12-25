using System.Collections.Generic;
using AudioVisual.DataStructures;
using AudioVisual.Utils;

namespace AudioVisual.Processor
{
    public class FrequencyBinAggregator : IProcessor
    {
        private List<int> _frequencyPartitionsSplits;

        private List<FftFrequencyBin> _fftValues;

        public List<double> GetAggregatedFrequencies(List<FftFrequencyBin> fftResult)
        {
            _fftValues?.Clear();
            _frequencyPartitionsSplits?.Clear();

            _fftValues = fftResult;
            _frequencyPartitionsSplits = MathUtils.SplitIntoNGeometricSeries(Constants.SegmentCount, (Constants.SampleRate / 4));

            var splitLowerBound = 0;
            var summedFrequencies = new List<double>(Constants.SegmentCount);

            for (var iSplit = 0; iSplit < Constants.SegmentCount; iSplit++)
            {
                var lowerFrequencyBoundary = splitLowerBound;
                var upperFrequencyBoundary = splitLowerBound + _frequencyPartitionsSplits[iSplit];
                var frequencyFilter =
                    new FrequencyFilter(_fftValues, lowerFrequencyBoundary, upperFrequencyBoundary);

                var sumOfSignificantAmplitudes = frequencyFilter.SumOfFiveBiggestContributingFrequencies();

                splitLowerBound += _frequencyPartitionsSplits[iSplit];

                summedFrequencies.Add(sumOfSignificantAmplitudes);
            }

            return summedFrequencies;
        }

        public List<double> GetFrequencies(List<double> fftResult)
        {
            return fftResult;
        }
    }
}
