using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CoAPExplorer.Models;
using CoAPExplorer.ViewModels;
using ReactiveUI;

namespace CoAPExplorer.WPF.Controls
{
    /// <summary>
    /// Interaction logic for DeviceListView.xaml
    /// </summary>
    public partial class DeviceListView : UserControl
    {
        public static readonly DependencyProperty DevicesProperty = DependencyProperty.Register(
            "Devices", typeof(IReactiveCollection<DeviceViewModel>), typeof(DeviceListView), new PropertyMetadata(default(List<DeviceViewModel>)));

        public IReactiveCollection<DeviceViewModel> Devices
        {
            get => (IReactiveCollection<DeviceViewModel>)GetValue(DevicesProperty);
            set => SetValue(DevicesProperty, value);
        }

        public DeviceListView()
        {
            InitializeComponent();

            DataContext = this;
        }
    }
}
