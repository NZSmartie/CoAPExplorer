using CoAPExplorer.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
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

namespace CoAPExplorer.WPF.Views
{
    /// <summary>
    /// Interaction logic for DeviceView.xaml
    /// </summary>
    public partial class DeviceView : UserControl, IViewFor<DeviceViewModel>
    {
        public DeviceView()
        {
            InitializeComponent();
        }

        #region IViewFor Boilerplate

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = value as DeviceViewModel;
        }

        public DeviceViewModel ViewModel { get; set; }

        #endregion
    }
}
