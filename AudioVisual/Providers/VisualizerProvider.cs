using System;
using System.Collections.Generic;
using AudioVisual.Processor;
using AudioVisual.Utils;
using AudioVisual.Visualizer;

namespace AudioVisual.Providers
{
    public static class VisualizerProvider
    {
        public static Dictionary<Enums.VisualizationMode, IProcessedDataVisualizer> RegisteredVisualizers =
            new Dictionary<Enums.VisualizationMode, IProcessedDataVisualizer>() {
            {Enums.VisualizationMode.Frequency, new FrequencyVisualizer(new FrequencyBinAggregator(), new FrequencySpectrumVisualizer())},
            {Enums.VisualizationMode.Circular, new CircularWaveVisualizer(new FilterBank(), new CircularFilterBankVisualizer())},
            {Enums.VisualizationMode.Circular, new WaveVisualizer(new FilterBank(), new FilterBankVisualizer())}

        };
        public static IProcessedDataVisualizer GetProcessedDataVisualizer(Enums.VisualizationMode type)
        {
            if (!RegisteredVisualizers.ContainsKey(type))
                throw new Exception("The requested ProcessedDataVisualizer does not exist!");

            return RegisteredVisualizers[type];
        }
    }
}
