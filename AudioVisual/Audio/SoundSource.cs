using System;
using CSCore.Codecs.MP3;

namespace AudioVisual
{
    public class SoundSource
    {
        public string FileName { get; set; }
        public DmoMp3Decoder Decoder { get; set; }

        public string SongName => FileName.Substring(0, FileName.Length - 4).Substring(6);

        public SoundSource(string fileName)
        {
            FileName = fileName;
            var filepath = fileName;

            Decoder = new DmoMp3Decoder(filepath) ?? throw new Exception("Error reading file " + fileName);
        }

        public override string ToString()
        {
            return SongName;
        }
    }
}