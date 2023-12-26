using AudioVisual.Utils;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AudioVisual.Settings
{
    public class ControlSettings : INotifyPropertyChanged
    {
        private bool _wavePartitionsBarVisibility;
        private Enums.VisualizationMode _visualizationMode = Constants.DefaultVisualizationMode;
        private int _wavePartitions = Constants.DefaultWavePartitions;

        public bool WavePartitionsBarVisibility
        {
            get => _wavePartitionsBarVisibility;
            set
            {
                _wavePartitionsBarVisibility = value;
                OnPropertyChanged();
            }
        }

        public Enums.VisualizationMode VisualizationMode
        {
            get => _visualizationMode;
            set
            {
                _visualizationMode = value;
                OnPropertyChanged();
            }
        }

        public int WavePartitions
        {
            get => _wavePartitions;
            set
            {
                _wavePartitions = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
