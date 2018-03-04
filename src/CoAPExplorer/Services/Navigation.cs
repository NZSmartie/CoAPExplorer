using CoAPExplorer.Models;
using CoAPExplorer.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoAPExplorer.Services
{
    public class Navigation
    {
        public NavigationViewModel ViewModel { get; }

        public IScreen HostScreen { get; set; }

        public Navigation()
        {
            ViewModel = new NavigationViewModel
            {
                NavigationItems = new ReactiveUI.ReactiveList<Models.NavigationItem>
                {
                    new NavigationItem {Name = "Favourites", Icon = CoapExplorerIcon.Favouriate},
                    new NavigationItem {
                        Name = "Recent",
                        Icon = CoapExplorerIcon.Recent,
                        Command = ReactiveCommand.Create(() => HostScreen?.Router.Navigate
                                                                                 .Execute(new RecentDevicesViewModel(HostScreen))
                                                                                 .Subscribe())
                    },
                    new NavigationItem {
                        Name = "Search",
                        Icon = CoapExplorerIcon.Search,
                        Command = ReactiveCommand.Create(() => HostScreen?.Router.Navigate
                                                                                 .Execute(new SearchViewModel(HostScreen))
                                                                                 .Subscribe())
                    },
                    new NavigationItem {
                        Name = "Settings",
                        Icon = CoapExplorerIcon.Settings,
                        Command = ReactiveCommand.Create(() => System.Diagnostics.Debug.WriteLine("TODO: Navigate to Setting ViewModel"))
                    }
                }
            };
        }
    }
}
