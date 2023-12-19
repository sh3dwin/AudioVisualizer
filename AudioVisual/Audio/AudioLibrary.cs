using System;
using System.Collections.Generic;

namespace AudioVisual.Audio
{
    public class AudioLibrary : ICloneable
    {
        public List<SoundSource> Library { get; private set; }

        public AudioLibrary(List<SoundSource> library)
        {
            Library = library;
        }

        public object Clone()
        {
            var list = new List<SoundSource>();
            foreach (var item in Library)
            {
                list.Add(item);
            }
            return new AudioLibrary(list);
        }
    }
}
