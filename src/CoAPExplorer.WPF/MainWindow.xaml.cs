using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
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
    public partial class MainWindow : Window, IViewFor<HomeViewModel>
    {
        public static readonly DependencyProperty IsNavigationFocusedProperty = DependencyProperty.Register(
            nameof(IsNavigationFocused), typeof(bool), typeof(MainWindow), new PropertyMetadata(true));

        public static readonly DependencyProperty PageTitleProperty = DependencyProperty.Register(
            nameof(PageTitle), typeof(string), typeof(MainWindow), new PropertyMetadata(string.Empty));

        public bool IsNavigationFocused
        {
            get => (bool)GetValue(IsNavigationFocusedProperty);
            set => SetValue(IsNavigationFocusedProperty, value);
        }

        public string PageTitle
        {
            get => (string)GetValue(PageTitleProperty);
            set => SetValue(PageTitleProperty, value);
        }

        object IViewFor.ViewModel { get => ViewModel; set => ViewModel = value as HomeViewModel; }
        public HomeViewModel ViewModel { get; set; }

        public MainWindow()
        {
            ViewModel = new HomeViewModel();

            InitializeComponent();

            NaigationList.SelectionChanged += NavigationChanged;

            this.WhenActivated(disposables =>
            {
                this.Bind(ViewModel, vm => vm.IsNavigationFocused, v => v.IsNavigationFocused)
                    .DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.PageTitle, v => v.PageTitle)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.Router, v => v.ViewHost.Router)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.Navigation, v => v.NavigationPane.DataContext)
                    .DisposeWith(disposables);
            });
        }



        private void NavigationChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var selected = e.AddedItems[0] as NavigationItem;
            Observable.Return(Unit.Default)
                      .InvokeCommand(selected.Command)
                      .Dispose();

            IsNavigationFocused = false;
        }
    }
}
