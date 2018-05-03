using CoAPExplorer.Models;
using CoAPExplorer.ViewModels;
using ReactiveUI;
using ReactiveUI.Routing;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : Page, IViewFor<HomeViewModel>
    {
        public IReactiveRouter Router { get; }

        public HomeViewModel ViewModel { get; set; }

        public HomeView(IReactiveRouter router = null)
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(x => x.ViewModel)
                    .Subscribe(vm => DataContext = vm)
                    .DisposeWith(disposables);
            });
        }


        object IViewFor.ViewModel { get => ViewModel; set => ViewModel = value as HomeViewModel; }
    }
}
