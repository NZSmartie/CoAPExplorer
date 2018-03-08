using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;

using CoAPExplorer.Models;
using CoAPExplorer.ViewModels;
using ReactiveUI;

namespace CoAPExplorer.WPF.Views
{
    /// <summary>
    /// Interaction logic for NavigationView.xaml
    /// </summary>
    public partial class NavigationView : UserControl, IViewFor<NavigationViewModel>
    {
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(
            nameof(IsOpen), typeof(bool), typeof(NavigationView), new PropertyMetadata(true));

        public static DependencyProperty ViewModelProperty = DependencyProperty.Register(
            nameof(ViewModel), typeof(NavigationViewModel), typeof(NavigationView), new PropertyMetadata(null));

        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }


        public NavigationViewModel ViewModel { get => GetValue(ViewModelProperty) as NavigationViewModel; set => SetValue(ViewModelProperty, value); }
        object IViewFor.ViewModel { get => ViewModel; set => ViewModel = value as NavigationViewModel; }

        public NavigationView()
        {
            InitializeComponent();

            //CollapseNavigation = ReactiveCommand.Create(() => IsOpen = false);

#if DEBUG
            if (DesignerProperties.GetIsInDesignMode(this))
                IsOpen = true;
#endif

            this.WhenActivated(disposables =>
            {
                this.Bind(ViewModel, vm => vm.IsOpen, v => v.IsOpen)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.NavigationItems, v => v.NaigationList.ItemsSource)
                    .DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.SelectedNavigationItem, v => v.NaigationList.SelectedItem)
                    .DisposeWith(disposables);

                //Observable.FromEventPattern<SelectionChangedEventHandler, SelectionChangedEventArgs>(
                //    h => NaigationList.SelectionChanged += h,
                //    h => NaigationList.SelectionChanged -= h)
                //    .SelectMany(x => x.EventArgs.AddedItems.Cast<NavigationItem>())
                //    .Select(n => Observable.Return(Unit.Default)
                //                           .InvokeCommand(n.Command)
                //                           .DisposeWith(disposables))
                //    .Subscribe()
                //    .DisposeWith(disposables);
            });
        }
    }
}
