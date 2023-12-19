using System;
using System.Windows;
using System.Windows.Controls;
using AudioVisual.Processor;
using AudioVisual.ViewModel;
using AudioVisual.Visualizer;

namespace AudioVisual.View
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

            _viewModel = new MainViewModel(
                new PlaybackViewModel(new Canvas()), new LoopbackViewModel(new Canvas()));
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