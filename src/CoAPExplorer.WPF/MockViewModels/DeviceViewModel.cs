using CoAPExplorer.Models;
using CoAPNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoAPExplorer.WPF.MockViewModels
{
    public class DeviceViewModel : CoAPExplorer.ViewModels.DeviceViewModel
    {
        public DeviceViewModel()
            :this(null)
        { }

        public DeviceViewModel(Device device)
            :base(device ?? new Device
            {
                Name = "Some Device",
                Endpoint = new CoapEndpoint(),
                Address = "192.168.x.x",
                LastSeen = DateTime.Now
            })
        {
            
        }
    }
}
