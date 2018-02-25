using System.Windows.Media.Media3D;
using CoAPExplorer.Models;
using MaterialDesignThemes.Wpf;
using ReactiveUI;

namespace CoAPExplorer.WPF.MockViewModels
{
    public class NavigationViewModel : CoAPExplorer.ViewModels.NavigationViewModel
    {
        public NavigationViewModel()
        {
            NavigationItems = new ReactiveList<NavigationItem>(new[]
            {
                new NavigationItem {Name = "Favourites", Icon = CoapExplorerIcon.Favouriate},
                new NavigationItem {Name = "Search", Icon = CoapExplorerIcon.Search},
                new NavigationItem {Name = "Settings", Icon = CoapExplorerIcon.Settings}
            });
        }
    }
}