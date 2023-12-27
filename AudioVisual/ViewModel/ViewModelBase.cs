using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AudioVisual.ViewModel
{
    public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public abstract void Dispose();
    }
}
