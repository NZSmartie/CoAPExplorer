using System;
using System.Collections.Generic;
using CoAPExplorer.Models;

namespace CoAPExplorer.Services
{
    public interface IDiscoveryService
    {
        IObservable<Device> DiscoverDevices();
        IObservable<DeviceResource> DiscoverResources(Device device);
        void SetFilters(IEnumerable<RequestFilter> filters);
        void SetRequstUrl(string Url);
        void SetTimeout(TimeSpan timeout);
    }
}