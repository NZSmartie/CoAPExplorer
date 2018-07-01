using System;
using System.Text;

using ReactiveUI;
using Splat;

namespace CoAPExplorer.ViewModels
{
    public class HomeViewModel : ReactiveObject, IRoutableViewModel
    {
        private SearchViewModel _search;
        private RecentDevicesViewModel _recentDevices;

        public RecentDevicesViewModel RecentDevices
        {
            get => _recentDevices ?? (_recentDevices = new RecentDevicesViewModel(HostScreen));
            set => _recentDevices = value;
        }

        public SearchViewModel Search
        {
            get => _search ?? (_search = new SearchViewModel(HostScreen));
            set => _search = value;
        }

        public IScreen HostScreen { get; }

        public string UrlPathSegment => "";

        public HomeViewModel(IScreen screen = null)
        {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();
        }
    }
}
