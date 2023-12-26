using System;
using System.Collections.Generic;
using AudioVisual.Utils;
using AudioVisual.Visualizer;

namespace AudioVisual.Providers
{
    public static class VisualizerProvider
    {
        public static Dictionary<Enums.VisualizationMode, IProcessedDataVisualizer> RegisteredVisualizers =
            new Dictionary<Enums.VisualizationMode, IProcessedDataVisualizer>() {
            {Enums.VisualizationMode.Frequency, new FrequencyVisualizer()},
            {Enums.VisualizationMode.Circular, new CircularWaveVisualizer()},
            {Enums.VisualizationMode.Wave, new WaveVisualizer()}

        };
        public static IProcessedDataVisualizer GetProcessedDataVisualizer(Enums.VisualizationMode type)
        {
            if (!RegisteredVisualizers.ContainsKey(type))
                throw new Exception("The requested ProcessedDataVisualizer does not exist!");

            return RegisteredVisualizers[type];
        }
    }
}
