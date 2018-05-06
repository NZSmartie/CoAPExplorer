using System;
using System.Collections.ObjectModel;
using CoAPExplorer.Models;
using CoAPExplorer.ViewModels;

namespace CoAPExplorer.WPF.MockViewModels
{
    public class DeviceListMock
    {
        public ObservableCollection<ViewModels.DeviceViewModel> Devices { get; }

        public DeviceListMock()
        {
            Devices = new ObservableCollection<ViewModels.DeviceViewModel>
            {
                new DeviceViewModel(new Device
                {
                    Name = "Garage Door",
                    Address = new Uri("coap://192.168.x.x/"),
                    LastSeen = DateTime.Now.AddMinutes(-123456)
                }),
                new DeviceViewModel(new Device{
                    Name = "Weather Station",
                    Address = new Uri("coap://192.168.x.x/"),
                    LastSeen = DateTime.Now.AddMinutes(-1),
                    IsFavourite = true,
                }),
                new DeviceViewModel(new Device{
                    Name = "Pet Food Station",
                    Address = new Uri("coap://192.168.x.x"),
                    LastSeen = DateTime.Now.AddMinutes(-12)
                })
            };
        }
    }
}