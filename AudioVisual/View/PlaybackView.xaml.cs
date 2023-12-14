using System.Windows.Controls;


namespace AudioVisual
{
    /// <summary>
    /// Interaction logic for PlaybackView.xaml
    /// </summary>
    public partial class PlaybackView : UserControl
    {
        private readonly PlaybackViewModel _viewModel;
        public PlaybackView()
        {
            InitializeComponent();
            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;

            _viewModel = new PlaybackViewModel(new AudioStreamProvider(), new AudioStreamPlayer(), new Canvas());
            DataContext = _viewModel;
            Loaded += PlaybackView_Loaded; ;
        }

        private async void PlaybackView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            await _viewModel.LoadAsync();
        }

        private void Dispatcher_ShutdownStarted(object sender, System.EventArgs e)
        {
            _viewModel.Dispose();
        }
    }
}
