using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Controls;

using ReactiveUI;
using Splat;

using CoAPExplorer.Models;
using CoAPExplorer.ViewModels;

namespace CoAPExplorer.WPF.Views
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : Page, IViewFor<HomeViewModel>
    {
        public IScreen Router { get; }

        public HomeViewModel ViewModel { get; set; }

        public HomeView(IScreen router = null)
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
