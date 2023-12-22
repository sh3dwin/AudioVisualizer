using System.Windows.Controls;
using AudioVisual.ViewModel;

namespace AudioVisual.View
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView
    {
        private readonly MainViewModel _viewModel;
        public MainView()
        {
            InitializeComponent();

            _viewModel = new MainViewModel(new Canvas());
            DataContext = _viewModel;

            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        }

        private void Dispatcher_ShutdownStarted(object sender, System.EventArgs e)
        {
            _viewModel.Dispose();
        }
    }
}
