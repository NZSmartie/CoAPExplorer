using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;

using MaterialDesignThemes.Wpf;
using ReactiveUI;
using Splat;

using CoAPExplorer.ViewModels;

namespace CoAPExplorer.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IScreen
    {
        public RoutingState Router { get; }

        public SnackbarMessageQueue ToastMessageQueue { get; }

        public MainWindow()
        {
            ToastMessageQueue = new SnackbarMessageQueue();

            InitializeComponent();

            // reactiveUI.Router stuff
            Router = new RoutingState();

            Router.CurrentViewModel.Subscribe(viewModel =>
            {
                if (viewModel == null)
                {
                    Frame.Content = null;
                    return;
                }

                var viewLocator = App.CoapExplorer.Locator.GetService<IViewLocator>();
                var view = viewLocator.ResolveView(viewModel);
                view.ViewModel = viewModel;
                Frame.Navigate(view);
            });

            Router.NavigateAndReset.Execute(new HomeViewModel(this))
                  .Subscribe();

            App.CoapExplorer.ToastNotifications.Subscribe(toast =>
            {
                var toastMessage = new SnackbarMessage { Content = toast.Message };

                if(toast.Actions.Count == 0)
                    ToastMessageQueue.Enqueue(toastMessage, true);
                else
                    ToastMessageQueue.Enqueue(
                        toastMessage, 
                        toast.Actions[0].Label.ToUpper(), p => ((ICommand)toast.Actions[0].Command).Execute(p),
                        null, 
                        false, true);

            });
        }
    }
}
