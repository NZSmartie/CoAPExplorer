using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Data;
using CoAPExplorer.Models;
using CoAPExplorer.Services;
using CoAPExplorer.ViewModels;
using CoAPExplorer.WPF.Views;
using ReactiveUI;
using Splat;

namespace CoAPExplorer.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IScreen
    {
        public static readonly DependencyProperty IsNavigationFocusedProperty = DependencyProperty.Register(
            nameof(IsNavigationFocused), typeof(bool), typeof(MainWindow), new PropertyMetadata(true));

        public static readonly DependencyProperty PageTitleProperty = DependencyProperty.Register(
            nameof(PageTitle), typeof(string), typeof(MainWindow), new PropertyMetadata(string.Empty));

        private static readonly DependencyProperty CollapseNavigationProperty = DependencyProperty.Register(
            nameof(CollapseNavigation), typeof(System.Windows.Input.ICommand), typeof(MainWindow), new PropertyMetadata(null));

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

        protected readonly CompositeDisposable CompositeDisposables = new CompositeDisposable();

        public RoutingState Router { get; } = new RoutingState();

        private ReactiveCommand CollapseNavigation
        {
            get => (ReactiveCommand)GetValue(CollapseNavigationProperty);
            set => SetValue(CollapseNavigationProperty, value);
        }

        public MainWindow()
        {
            CollapseNavigation = ReactiveCommand.Create(() => IsNavigationFocused = false, this.WhenAnyValue(vm => vm.IsNavigationFocused));

            InitializeComponent();

            ViewHost.Router = Router;

            Router.NavigateAndReset
                  .Execute(new HomeViewModel(this))
                  .Subscribe();

            Router.CurrentViewModel
                  .Subscribe(rvm =>
                  {
                      PageTitle = rvm.UrlPathSegment;

                      if (rvm is ISupportsNavigatation nvm)
                      {
                          var view = ViewLocator.Current.ResolveView(nvm.Navigation);
                          NavigationPane.Content = view;
                          view.ViewModel = nvm.Navigation;

                          ClearValue(IsNavigationFocusedProperty);
                          SetBinding(IsNavigationFocusedProperty, new Binding("IsOpen") { Source = nvm.Navigation, Mode = BindingMode.TwoWay });
                      }
                  })
                  .DisposeWith(CompositeDisposables);

#if DEBUG
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                IsNavigationFocused = true;
            }
#endif
        }

        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);

            CompositeDisposables.Dispose();
        }
    }
}
