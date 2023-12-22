﻿using AudioVisual.Processor;
using System.Windows.Controls;
using NAudio.Dsp;
using AudioVisual.DataStructures;
using System.Collections.Generic;

namespace AudioVisual.Visualizer
{
    public class CircularWaveVisualizer : IProcessedFrequencySpectrumVisualizer
    {
        private readonly SubBandFilterBank _processor;
        private readonly CircularSubBandFilterBankVisualizer _visualizer;

        public CircularWaveVisualizer(SubBandFilterBank processor, CircularSubBandFilterBankVisualizer visualizer)
        {
            _processor = processor;
            _visualizer = visualizer;

        }

        public Canvas Draw(List<FftFrequencyBin> frequencySpectrum)
        {
            var filterBank = _processor.GetSubBandFilterBank(frequencySpectrum);
            return _visualizer.Draw(filterBank);
        }

        public void SetBandPassCount(int bandPassCount)
        {
            _processor.BandPassCount = bandPassCount;
        }

        public void Dispose()
        {
            _visualizer?.Dispose();
        }
    }
}
