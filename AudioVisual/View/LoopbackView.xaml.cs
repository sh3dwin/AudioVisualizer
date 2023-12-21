using System.Windows.Controls;
using AudioVisual.Processor;
using AudioVisual.ViewModel;
using AudioVisual.Visualizer;

namespace AudioVisual
{
    /// <summary>
    /// Interaction logic for LoopbackView.xaml
    /// </summary>
    public partial class LoopbackView : UserControl
    {
        private LoopbackViewModel _viewModel;
        public LoopbackView()
        {
            InitializeComponent();

            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;

            _viewModel = new LoopbackViewModel(new Canvas());
            DataContext = _viewModel;
            Loaded += PlaybackView_Loaded; ;

            Visualizer.Dispatcher.Invoke(() =>
            {

            });
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
