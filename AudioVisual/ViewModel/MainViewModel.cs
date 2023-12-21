namespace AudioVisual.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel(LoopbackViewModel loopbackViewModel)
        {
            LoopbackViewModel = loopbackViewModel;
        }

        public LoopbackViewModel LoopbackViewModel { get; }

        public override void Dispose()
        {
            LoopbackViewModel.Dispose();
        }
    }
}
