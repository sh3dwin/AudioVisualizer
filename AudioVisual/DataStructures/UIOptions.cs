using System;
using AudioVisual.Utils;
using System.Collections.Generic;

namespace AudioVisual.DataStructures
{
    public class UiOptions
    {
        // Options
        public int WavePartitions;
        public bool WavePartitionsBarVisibility;
        public Enums.VisualizationMode VisualizationMode;

        private readonly Dictionary<Enums.VisualizationMode, bool> _barVisibility = new()
        {
            {Enums.VisualizationMode.CircularWave, true},
            {Enums.VisualizationMode.Wave, true},
            {Enums.VisualizationMode.Frequency, false},
            {Enums.VisualizationMode.CircularFrequency, false}
        };

        public UiOptions(
            int wavePartitions = Constants.DefaultWavePartitions,
            bool wavePartitionsBarVisibility = Constants.DefaultWavePartitionsBarVisibility,
            Enums.VisualizationMode visualizationMode = Constants.DefaultVisualizationMode)
        {
            WavePartitions = wavePartitions;
            WavePartitionsBarVisibility = wavePartitionsBarVisibility;
            VisualizationMode = visualizationMode;
        }

        public void Update()
        {
            if (!_barVisibility.TryGetValue(VisualizationMode, out var value))
                throw new Exception("The VisualizationMode does not exist!");
            
            WavePartitionsBarVisibility = value;
        }
    }
}
