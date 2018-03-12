using CoAPExplorer.Models;
using CoAPExplorer.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;

namespace CoAPExplorer.Services
{
    public class Navigation : ReactiveObject
    {
        private HomeViewModel _homeView;

        public NavigationViewModel ViewModel { get; }

        public IScreen HostScreen { get; set; }

        public HomeViewModel HomeView { get => _homeView; set => this.RaiseAndSetIfChanged(ref _homeView, value); }

        public Navigation()
        {
            ViewModel = new NavigationViewModel
            {
                NavigationItems = new ReactiveUI.ReactiveList<Models.NavigationItem>
                {
                    new NavigationItem {
                        Name = "Recent",
                        Icon = CoapExplorerIcon.Recent,
                        Command = ReactiveCommand.Create(() => HomeView != null ? HomeView.Router.NavigateAndReset
                                                                                 .Execute(new RecentDevicesViewModel(HostScreen))
                                                                                 .Subscribe() : Disposable.Empty),
                    },
                    new NavigationItem {
                        Name = "Search",
                        Icon = CoapExplorerIcon.Search,
                        Command = ReactiveCommand.Create(() => HomeView != null ? HomeView.Router.NavigateAndReset
                                                                                 .Execute(new SearchViewModel(HostScreen))
                                                                                 .Subscribe() : Disposable.Empty),
                    },
                    new NavigationItem {
                        Name = "Settings",
                        Icon = CoapExplorerIcon.Settings,
                        Command = ReactiveCommand.Create(() => System.Diagnostics.Debug.WriteLine("TODO: Navigate to Setting ViewModel")),
                    }
                }
            };
        }
    }
}
