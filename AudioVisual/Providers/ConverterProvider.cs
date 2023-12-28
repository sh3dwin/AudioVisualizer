using System;
using System.Collections.Generic;
using AudioVisual.AudioVisualConverters;
using AudioVisual.Utils;

namespace AudioVisual.Providers
{
    public static class ConverterProvider
    {
        public static Dictionary<Enums.VisualizationMode, IFilterBankConverter> RegisteredConverters =
            new() {
                {Enums.VisualizationMode.Frequency, new FrequencyConverter()},
                {Enums.VisualizationMode.CircularWave, new CircularWaveConverter()},
                {Enums.VisualizationMode.Wave, new WaveConverter()},
                {Enums.VisualizationMode.CircularFrequency, new CircularFrequencyConverter()}

            };
        public static IFilterBankConverter GetConverter(Enums.VisualizationMode type)
        {
            if (!RegisteredConverters.ContainsKey(type))
                throw new Exception("The requested Converter does not exist!");

            return RegisteredConverters[type];
        }
    }
}
