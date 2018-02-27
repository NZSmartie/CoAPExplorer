using System;
using System.Collections.Generic;
using System.Globalization;
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
using CoAPExplorer.Models;
using CoAPExplorer.ViewModels;
using ReactiveUI;

namespace CoAPExplorer.WPF.Views
{
    /// <summary>
    /// Interaction logic for SearchView.xaml
    /// </summary>
    public partial class SearchView : UserControl, IViewFor<SearchViewModel>
    {

        public SearchView()
        {
            InitializeComponent();

            this.WhenActivated((CompositeDisposable disposables) =>
            {
                var visibilityConverter = new BooleanToVisibilityConverter();

                this.Bind(ViewModel, vm => vm.Devices, v => v.DeviceList.Devices)
                    .DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.SearchUrl, v => v.SearchUrl.Text)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel, 
                        vm => vm.IsSearching, 
                        v => v.GoButton.Visibility, 
                        x => visibilityConverter.Convert(!x, typeof(Visibility), null, CultureInfo.CurrentCulture))
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.SearchCommand, v => v.GoButton)
                    .DisposeWith(disposables);


                this.OneWayBind(ViewModel, 
                        vm => vm.IsSearching, 
                        v => v.StopButton.Visibility, 
                        x => visibilityConverter.Convert(x, typeof(Visibility), null, CultureInfo.CurrentCulture))
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.StopCommand, v => v.StopButton)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel, 
                        vm => vm.IsSearching, 
                        v => v.SearchProgress.Visibility, 
                        x => visibilityConverter.Convert(x, typeof(Visibility), null, CultureInfo.CurrentCulture))
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.IsSearching, v => v.FilterPanel.IsEnabled, x => !x)
                    .DisposeWith(disposables);
            });
        }

        #region IViewFor Boilerplate

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = value as SearchViewModel;
        }

        public SearchViewModel ViewModel { get; set; }

        #endregion
    }
}
