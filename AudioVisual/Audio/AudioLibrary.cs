using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioVisual
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
