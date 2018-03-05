using System;
using System.Reactive.Linq;
using System.Linq;
using System.Windows.Controls;
using System.Reactive.Disposables;

using MaterialDesignThemes.Wpf;
using ReactiveUI;

using CoAPExplorer.ViewModels;

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

                ViewModel.AddDeviceCommand.Subscribe(ndvm =>
                                           {
                                               System.Diagnostics.Debug.WriteLine("Opening Dialog window");
                                               var view = ViewLocator.Current.ResolveView(ndvm);
                                               view.ViewModel = ndvm;

                                               DialogHost.Show(view);
                                           })
                                          .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.AddDeviceCommand, v => v.AddButton, nameof(AddButton.ToggleCheckedContentClick))
                    .DisposeWith(disposables);

                AddButton.WhenAnyValue(a => a.IsPopupOpen).Subscribe(isOpen => System.Diagnostics.Debug.WriteLine($"popup is {isOpen}")).DisposeWith(disposables);
            });
        }

        public RecentDevicesViewModel ViewModel { get; set; }
        object IViewFor.ViewModel { get => ViewModel; set => ViewModel = value as RecentDevicesViewModel; }
    }
}
