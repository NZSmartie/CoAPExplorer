using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Linq;
using System.Windows.Controls;
using System.Reactive.Disposables;

using MaterialDesignThemes.Wpf;
using ReactiveUI;

using CoAPExplorer.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace CoAPExplorer.WPF.Views
{
    /// <summary>
    /// Interaction logic for RecentDevicesView.xaml
    /// </summary>
    public partial class RecentDevicesView : UserControl, IViewFor<RecentDevicesViewModel>
    {
        private static readonly DependencyProperty SearchCommandProperty = DependencyProperty.Register(
            nameof(SearchCommand), typeof(ReactiveCommand), typeof(RecentDevicesView), new PropertyMetadata(default(ReactiveCommand)));

        private ReactiveCommand SearchCommand { get => GetValue(SearchCommandProperty) as ReactiveCommand; set => SetValue(SearchCommandProperty, value); }

        public RecentDevicesView()
        {
            InitializeComponent();

            SearchCommand = ReactiveCommand.Create(() => 
            {
                AppBarTransistioner.SelectedItem = SearchTransistionState;
                SearchTextBox.Focus();
            });

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.FilteredDevices, v => v.DeviceListView.Devices)
                    .DisposeWith(disposables);

                this.WhenAnyValue(x => x.ViewModel)
                    .Where(x => x != null)
                    .Subscribe(vm => vm.AddDeviceCommand.Subscribe(ndvm =>
                        {
                            var view = ViewLocator.Current.ResolveView(ndvm);
                            view.ViewModel = ndvm;

                            DialogHost.Show(view);
                        })
                        .DisposeWith(disposables))
                    .DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.SearchTerms, v => v.SearchTextBox.Text)
                    .DisposeWith(disposables);

                this.SearchTextBox.Events()
                    .KeyDown.Where(k => k.Key == Key.Enter && ViewModel?.NavigateToUriCommand != null)
                    .Select(_ => Unit.Default).InvokeCommand(this, v => v.ViewModel.NavigateToUriCommand)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.NavigateToUriCommand, v => v.NavigateToButton)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel,
                                vm => vm.IsSearchValidUri, 
                                v => v.NavigateToButton.Visibility, 
                                x => x ? Visibility.Visible : Visibility.Collapsed)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.AddDeviceCommand, v => v.AddButton, nameof(AddButton.ToggleCheckedContentClick))
                    .DisposeWith(disposables);
            });
        }

        public readonly static DependencyProperty ViewModelProperty = DependencyProperty.Register(
            nameof(ViewModel), typeof(RecentDevicesViewModel), typeof(RecentDevicesView), new PropertyMetadata(null));

        public RecentDevicesViewModel ViewModel { get => (RecentDevicesViewModel)GetValue(ViewModelProperty); set => SetValue(ViewModelProperty, value); }

        object IViewFor.ViewModel { get => ViewModel; set => ViewModel = value as RecentDevicesViewModel; }
    }
}
