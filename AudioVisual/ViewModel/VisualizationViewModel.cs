using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace AudioVisual
{
    public class VisualizationViewModel : INotifyPropertyChanged, IDisposable
    {
        public VisualizationViewModel()
        {
        }

        public void Dispose()
        {
            // TODO release managed resources here
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
