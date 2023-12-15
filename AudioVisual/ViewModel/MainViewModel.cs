using System;
using System.Threading.Tasks;
using AudioVisual.Command;

namespace AudioVisual
{
    public class MainViewModel : ViewModelBase, IDisposable
    {
        private ViewModelBase _selectedViewModel;

        public MainViewModel(PlaybackViewModel playbackViewModel, LoopbackViewModel loopbackViewModel)
        {
            PlaybackViewModel = playbackViewModel;
            LoopbackViewModel = loopbackViewModel;
            SelectedViewModel = loopbackViewModel;

            SelectViewModelCommand = new DelegateCommand(SelectViewModel);
        }

        public DelegateCommand SelectViewModelCommand { get; }

        public PlaybackViewModel PlaybackViewModel { get; }
        public LoopbackViewModel LoopbackViewModel { get; }

        public ViewModelBase? SelectedViewModel
        {
            get => _selectedViewModel;
            set
            {
                if (_selectedViewModel is not null)
                    _selectedViewModel.Dispose();
                _selectedViewModel = value;
                RaisePropertyChanged();
            }
        }

        public override async Task LoadAsync()
        {
            if (SelectedViewModel is not null)
            {
                await SelectedViewModel.LoadAsync();
            }
        } 
        public void Dispose()
        {
            PlaybackViewModel.Dispose();
            LoopbackViewModel.Dispose();
        }

        private void SelectViewModel(object? parameter)
        {
            SelectedViewModel = parameter as ViewModelBase;
        }


    }
}
