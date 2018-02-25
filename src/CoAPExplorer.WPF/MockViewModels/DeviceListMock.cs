using System;
using System.Collections.ObjectModel;
using CoAPExplorer.Models;

namespace CoAPExplorer.WPF.MockViewModels
{
    public class DeviceListMock
    {
        public ObservableCollection<Device> Devices { get; }

        public DeviceListMock()
        {
            Devices = new ObservableCollection<Device>
            {
                new Device
                {
                    Name = "Garage Door",
                    Address = "192.168.x.x",
                    LastSeen = DateTime.Now.AddMinutes(-123456)
                },
                new Device{
                    Name = "Weather Station",
                    Address = "192.168.x.x",
                    LastSeen = DateTime.Now.AddMinutes(-1)
                }
                ,
                new Device{
                    Name = "Pet Food Station",
                    Address = "192.168.x.x",
                    LastSeen = DateTime.Now.AddMinutes(-12)
                }
            };
        }
    }
}