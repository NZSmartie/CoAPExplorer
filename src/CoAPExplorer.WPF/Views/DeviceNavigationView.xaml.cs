using CoAPExplorer.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
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
    /// Interaction logic for DeviceNavigationView.xaml
    /// </summary>
    public partial class DeviceNavigationView : UserControl, IViewFor<DeviceNavigationViewModel>
    {
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(
            nameof(IsOpen), typeof(bool), typeof(DeviceNavigationView), new PropertyMetadata(true));

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            nameof(ViewModel), typeof(DeviceNavigationViewModel), typeof(DeviceNavigationView), new PropertyMetadata(null));

        public DeviceNavigationViewModel ViewModel { get => GetValue(ViewModelProperty) as DeviceNavigationViewModel; set => SetValue(ViewModelProperty, value); }
        object IViewFor.ViewModel { get => ViewModel; set => ViewModel = value as DeviceNavigationViewModel; }

        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }

        public DeviceNavigationView()
        {
            InitializeComponent();

            //SetBinding(DataContextProperty, new Binding(nameof(ViewModel)) { Mode = BindingMode.OneWay });
            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(t => t.ViewModel).Subscribe(NewViewModel => DataContext = NewViewModel).DisposeWith(disposables);

                this.BindCommand(ViewModel, 
                                 vm => vm.RefreshResourcesCommand, 
                                 v => v.RefreshButton, 
                                 vm => vm.Device)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.Resources, v => v.ResourceList.ItemsSource)
                    .DisposeWith(disposables);
            });
        }
    }
}
