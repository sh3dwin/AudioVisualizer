using System.Collections.Generic;
using AudioVisual.DataStructures;
using AudioVisual.Utils;

namespace AudioVisual.Processor
{
    public class FilterBank : IProcessor
    {
        private List<int> _bandPassSplits;
        private int _bandPassCount;

        private List<FftFrequencyBin> _fftValues;
        
        public FilterBank(int bandPassCount = 3)
        {
            _bandPassCount = bandPassCount;
            
            var maxFrequency = (int)(Constants.SampleRate * 0.25);
            _bandPassSplits = MathUtils.SplitIntoNGeometricSeries(_bandPassCount, maxFrequency);
        }
        public int BandPassCount
        {
            get => _bandPassCount;
            set
            {
                _bandPassCount = value;
                var maxFrequency = (int)(Constants.SampleRate * 0.25);
                _bandPassSplits =
                    MathUtils.SplitIntoNGeometricSeries(_bandPassCount, maxFrequency);
            }
        }

        public List<FrequencyFilter> GetFilterBank(List<FftFrequencyBin> fftResult)
        {
            _fftValues = fftResult;

            var splitLowerBound = 0;
            var allFrequencyWindows = new List<FrequencyFilter>(_bandPassCount + 1);

            // If the number of partitions is only one, return only the whole wave
            if (BandPassCount == 1)
            {
                return new List<FrequencyFilter>
                {
                    new(_fftValues, 0, (Constants.SampleRate * 0.25))
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
                new FrequencyFilter(_fftValues, 1, (Constants.SampleRate * 0.25));
            allFrequencyWindows.Add(wholeWave);

            return allFrequencyWindows;
        }
    }
}
