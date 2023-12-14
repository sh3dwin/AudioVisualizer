using System;
using System.Windows;
using System.Windows.Controls;

namespace AudioVisual
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        private readonly MainViewModel _viewModel;
        public MainView()
        {
            InitializeComponent();
            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;

            _viewModel = new MainViewModel(new PlaybackViewModel(
            new AudioStreamProvider(), new AudioStreamPlayer(), new Canvas()),
                new LoopbackViewModel(new Canvas()));
            DataContext = _viewModel;
            Loaded += MainView_Loaded;
        }

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            _viewModel.Dispose();
        }

        private async void MainView_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadAsync();
        }
    }
}