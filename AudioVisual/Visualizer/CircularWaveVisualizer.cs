using AudioVisual.Processor;
using System.Windows.Controls;
using AudioVisual.DataStructures;
using System.Collections.Generic;
using AudioVisual.Analysis;
using AudioVisual.Utils;

namespace AudioVisual.Visualizer
{
    public class CircularWaveVisualizer : IProcessedDataVisualizer
    {
        private readonly FilterBank _processor = new();
        private readonly CircularFilterBankVisualizer _visualizer = new();
        private readonly FourierTransformAnalyzer _analyzer = new();

        public Canvas Draw(Canvas canvas, List<FftFrequencyBin> frequencySpectrum, int wavePartitionsCount)
        {
            var filterBank = _processor.GetFrequencyFilters(frequencySpectrum, wavePartitionsCount);
            var waves = new List<List<double>>();
            foreach (var filter in filterBank)
            {
                waves.Add(_analyzer.Synthesize(filter, Constants.PowerOfTwo));
            }
            return _visualizer.Draw(canvas, waves);
        }

        public void Dispose()
        {
            _visualizer?.Dispose();
        }
    }
}
