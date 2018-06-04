using System;
using System.Reactive.Linq;
using System.Windows;

using MaterialDesignThemes.Wpf;
using ReactiveUI.Routing;
using ReactiveUI.Routing.WPF;

using CoAPExplorer.ViewModels;
using ReactiveUI;
using System.Windows.Input;

namespace CoAPExplorer.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public IReactiveRouter Router { get; }

        public SnackbarMessageQueue ToastMessageQueue { get; }

        public MainWindow()
        {
            ToastMessageQueue = new SnackbarMessageQueue();

            InitializeComponent();

            // reactiveUI.Router stuff
            Router = App.CoapExplorer.Router;
            PagePresenter.RegisterHost(Frame);

            Router.Navigate(NavigationRequest.Reset() + NavigationRequest.Forward(new HomeViewModel(Router)))
                  .Subscribe();

            App.CoapExplorer.ToastNotifications.Subscribe(toast =>
            {
                var toastMessage = new SnackbarMessage { Content = toast.Message };

                if(toast.Actions.Count == 0)
                    ToastMessageQueue.Enqueue(toastMessage, true);
                else
                    ToastMessageQueue.Enqueue(toastMessage, toast.Actions[0].Label, _ => ((ICommand)toast.Actions[0].Command).Execute(null), null, false, true);

            });
        }
    }
}
