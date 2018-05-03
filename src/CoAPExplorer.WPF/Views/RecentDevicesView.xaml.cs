using System;
using System.Reactive.Linq;
using System.Linq;
using System.Windows.Controls;
using System.Reactive.Disposables;

using MaterialDesignThemes.Wpf;
using ReactiveUI;

using CoAPExplorer.ViewModels;
using System.Windows;

namespace CoAPExplorer.WPF.Views
{
    /// <summary>
    /// Interaction logic for RecentDevicesView.xaml
    /// </summary>
    public partial class RecentDevicesView : UserControl, IViewFor<RecentDevicesViewModel>
    {
        public RecentDevicesView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.Devices, v => v.DeviceListView.Devices)
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
