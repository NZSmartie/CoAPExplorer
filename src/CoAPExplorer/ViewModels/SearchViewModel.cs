using System;
using System.Collections.ObjectModel;
using CoAPExplorer.Models;
using ReactiveUI;

namespace CoAPExplorer.ViewModels
{
    public class SearchViewModel : ReactiveObject, IRoutableViewModel
    {
        public string UrlPathSegment => "Device Search";

        public IScreen HostScreen { get; }

        public ObservableCollection<Device> Devices { get; }

        public SearchViewModel(IScreen hostScreen)
        {
            HostScreen = hostScreen ?? throw new ArgumentNullException(nameof(hostScreen));

            Devices = new ObservableCollection<Device>();
        }
    }
}