using System;
using System.Collections.Generic;
using AudioVisual.AudioVisualConverters;
using AudioVisual.Visualizer;

namespace AudioVisual.Providers
{
    public static class VisualizerProvider
    {
        public static Dictionary<Type, VisualizerAbstract> RegisteredVisualizers =
            new() {
            {typeof(FrequencyConverter), new FrequencySpectrumVisualizer()},
            {typeof(WaveConverter), new WaveVisualizer()},
            {typeof(CircularWaveConverter), new CircularWaveVisualizer()}

        };
        public static IVisualizer GetVisualizer(IFilterBankConverter converter)
        {
            var type = converter.GetType();
            if (!RegisteredVisualizers.ContainsKey(type))
                throw new Exception("The requested Visualizer does not exist!");

            return RegisteredVisualizers[type];
        }
    }
}
