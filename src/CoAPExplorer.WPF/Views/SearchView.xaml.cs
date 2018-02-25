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
                this.Bind(ViewModel, vm => vm.Devices, v => v.DeviceList.Devices)
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
