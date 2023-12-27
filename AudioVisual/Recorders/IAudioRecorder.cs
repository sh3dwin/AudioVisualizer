using System;
using NAudio.Wave;

namespace AudioVisual.Recorders
{
    public interface IAudioRecorder : IDisposable
    {
        public event EventHandler DataAvailableEvent;
        public WaveBuffer GetAudioData();
    }
}
