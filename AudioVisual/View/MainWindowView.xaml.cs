using System;
using System.Windows;
using System.Windows.Controls;

namespace AudioVisual
{
    /// <summary>
    /// Interaction logic for MainWindowView.xaml
    /// </summary>
    public partial class MainWindowView : Window
    {
        private readonly MainWindowViewModel _viewModel;
        public MainWindowView()
        {
            InitializeComponent();
            Loaded += NowPlayingView_Loaded;
            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;

            _viewModel = new MainWindowViewModel(new AudioStreamProvider(), new AudioStreamPlayer(), new Canvas());
            DataContext = _viewModel;
        }

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            _viewModel.Dispose();
        }

        private async void NowPlayingView_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadAsync();
        }
    }
}