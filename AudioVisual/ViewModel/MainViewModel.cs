using System;
using System.Threading.Tasks;

namespace AudioVisual
{
    public class MainViewModel : ViewModelBase, IDisposable
    {
        private ViewModelBase _selectedViewModel;

        public MainViewModel(PlaybackViewModel playbackViewModel, LoopbackViewModel loopbackViewModel)
        {
            PlaybackViewModel = playbackViewModel;
            LoopbackViewModel = loopbackViewModel;
            SelectedViewModel = playbackViewModel;
        }

        public PlaybackViewModel PlaybackViewModel;
        public LoopbackViewModel LoopbackViewModel;

        public ViewModelBase? SelectedViewModel
        {
            get => _selectedViewModel;
            set
            {
                _selectedViewModel = value;
                RaisePropertyChanged();
            }
        }

        public override async Task LoadAsync()
        {
            await SelectedViewModel.LoadAsync();
        } 
        public void Dispose()
        {
            PlaybackViewModel.Dispose();
            LoopbackViewModel.Dispose();
        }


    }
}
