using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioVisual
{
    public static class AudioLibraryFactory
    {
        public static AudioLibrary CreateLibrary(string filepath)
        {
            return new AudioStreamProvider().GetAllSongs();
        }
    }
}
