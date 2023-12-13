using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioVisual
{
    public interface ISongProvider
    {
        AudioLibrary GetAllSongs(string path = null);
    }
    public class AudioStreamProvider : ISongProvider
    {
        public AudioLibrary GetAllSongs(string path = null)
        {
            var directory = path ?? "Audio/";
            List<SoundSource> songs = new List<SoundSource>();
            foreach (string file in Directory.EnumerateFiles(directory))
            {
                songs.Add(new SoundSource(file));
            }
            return new AudioLibrary(songs);
        }
    }
}