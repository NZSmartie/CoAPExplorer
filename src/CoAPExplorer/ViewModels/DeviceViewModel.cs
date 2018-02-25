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
        private readonly CoapEndpoint _endpoint;

        private string _urlPathSegment;
        public string UrlPathSegment => _urlPathSegment ?? (_urlPathSegment = _endpoint.BaseUri.ToString());

        public Device Device { get; }

        public IScreen HostScreen { get; private set; }

        public DeviceViewModel(CoapEndpoint endpoint, Device device = null, IScreen hostScreen = null)
        {
            _endpoint = endpoint;
            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>()
                ?? throw new InvalidOperationException($"Could not locate a {nameof(IScreen)} service");

            Device = device ?? new Device();
        }
    }
}
