using System;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Collections.Generic;
using System.Text;

using ReactiveUI;
using CoAPExplorer.Services;
using Splat;
using System.Reactive;
using CoAPExplorer.Models;
using ReactiveUI.Routing;

namespace CoAPExplorer.ViewModels
{
    public class HomeViewModel : ReactiveObject
    {
        private SearchViewModel _search;
        private RecentDevicesViewModel _recentDevices;

        public RecentDevicesViewModel RecentDevices
        {
            get => _recentDevices ?? (_recentDevices = new RecentDevicesViewModel(Router));
            set => _recentDevices = value;
        }

        public SearchViewModel Search
        {
            get => _search ?? (_search = new SearchViewModel(Router));
            set => _search = value;
        }

        public IReactiveRouter Router { get; }

        public HomeViewModel(IReactiveRouter router = null)
        {
            Router = router ?? Locator.Current.GetService<IReactiveRouter>();
        }
    }
}
