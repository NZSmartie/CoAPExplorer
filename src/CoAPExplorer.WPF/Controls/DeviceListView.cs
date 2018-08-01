using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using ReactiveUI;

using CoAPExplorer.ViewModels;

namespace CoAPExplorer.WPF.Controls
{
    public partial class DeviceListView : ListView
    {
        public static readonly DependencyProperty DevicesProperty = DependencyProperty.Register(
            "Devices", typeof(IReactiveCollection<DeviceViewModel>), typeof(DeviceListView), new PropertyMetadata(default(List<DeviceViewModel>)));

        public IReactiveCollection<DeviceViewModel> Devices
        {
            get => (IReactiveCollection<DeviceViewModel>)GetValue(DevicesProperty);
            set => SetValue(DevicesProperty, value);
        }
    }
}
