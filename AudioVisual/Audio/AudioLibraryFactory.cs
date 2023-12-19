namespace AudioVisual.Audio
{
    public static class AudioLibraryFactory
    {
        public static AudioLibrary CreateLibrary(string filepath)
        {
            return new AudioStreamProvider().GetAllSongs();
        }
    }
}
