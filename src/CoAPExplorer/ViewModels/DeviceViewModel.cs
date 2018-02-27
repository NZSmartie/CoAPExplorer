using System;
using System.Collections.Generic;
using System.Text;
using CoAPExplorer.Models;
using CoAPNet;
using ReactiveUI;
using Splat;

namespace CoAPExplorer.ViewModels
{
    public class DeviceViewModel : ReactiveObject, IRoutableViewModel
    {
        private string _urlPathSegment;

        public string UrlPathSegment => _urlPathSegment ?? (_urlPathSegment = Endpoint.BaseUri.ToString());

        public Device Device { get; }

        public IScreen HostScreen { get; private set; }

        public ICoapEndpoint Endpoint => Device.Endpoint;

        public string Name => Device.Name;

        public string Address => Device.Address;

        public DateTime LastSeen => Device.LastSeen;

        public ReactiveCommand OpenCommand { get; }

        public DeviceViewModel(Device device = null, IScreen hostScreen = null)
        {
            HostScreen = hostScreen;
            Device = device ?? new Device();

            OpenCommand = ReactiveCommand.Create(() => hostScreen.Router.Navigate.Execute(this).Subscribe());
        }
    }
}
