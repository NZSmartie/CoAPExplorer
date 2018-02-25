using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;

using CoAPExplorer.Models;
using CoAPExplorer.Services;
using CoAPExplorer.ViewModels;
using ReactiveUI;
using Splat;

namespace CoAPExplorer.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IScreen
    {
        public RoutingState Router { get; set; }

        private readonly Navigation _navigation;

        public MainWindow()
        {
            _navigation = App.CoapExplorer.Services.GetService<Navigation>();
            Router = new RoutingState();
            // TODO: Set Favourites? or last viewed?
            Router.NavigateAndReset
                .Execute(new SearchViewModel(this))
                .Subscribe();

            InitializeComponent();

            ViewHost.Router = Router;

            NavigationPane.DataContext = _navigation.ViewModel;

            NaigationList.SelectionChanged += NavigationChanged;
        }

        private void NavigationChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var selected = e.AddedItems[0] as NavigationItem;
            Observable.Return(Unit.Default)
                      .InvokeCommand(selected.Command)
                      .Dispose();
        }
    }
}
