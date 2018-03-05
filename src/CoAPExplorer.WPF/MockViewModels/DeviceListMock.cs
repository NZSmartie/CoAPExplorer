using System;
using System.Collections.ObjectModel;
using CoAPExplorer.Models;
using CoAPExplorer.ViewModels;

namespace CoAPExplorer.WPF.MockViewModels
{
    public class DeviceListMock
    {
        public ObservableCollection<DeviceViewModel> Devices { get; }

        public DeviceListMock()
        {
            Devices = new ObservableCollection<DeviceViewModel>
            {
                new DeviceViewModel(new Device
                {
                    Name = "Garage Door",
                    Address = "192.168.x.x",
                    LastSeen = DateTime.Now.AddMinutes(-123456)
                }),
                new DeviceViewModel(new Device{
                    Name = "Weather Station",
                    Address = "192.168.x.x",
                    LastSeen = DateTime.Now.AddMinutes(-1),
                    IsFavourite = true,
                }),
                new DeviceViewModel(new Device{
                    Name = "Pet Food Station",
                    Address = "192.168.x.x",
                    LastSeen = DateTime.Now.AddMinutes(-12)
                })
            };
        }
    }
}