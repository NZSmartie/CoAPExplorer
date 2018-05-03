using System;
using System.Reactive.Linq;
using System.Windows;
using CoAPExplorer.ViewModels;

using ReactiveUI.Routing;
using ReactiveUI.Routing.WPF;

namespace CoAPExplorer.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public IReactiveRouter Router { get; }

        public MainWindow()
        {
            InitializeComponent();

            // reactiveUI.Router stuff
            Router = App.CoapExplorer.Router;
            PagePresenter.RegisterHost(Frame);

            Router.Navigate(NavigationRequest.Reset() + NavigationRequest.Forward(new HomeViewModel(Router)))
                  .Subscribe();
        }
    }
}
