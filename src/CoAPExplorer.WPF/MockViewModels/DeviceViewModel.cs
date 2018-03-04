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
            Message = new Message
            {
                Id = 1234,
                Token = new byte[] { 0x01, 0x02, 0x03, 0x04 },

                Url = "/some/resource",
                Code = CoapMessageCode.Get,
                //Type = CoapMessageType.Confirmable,

                ContentFormat = CoAPNet.Options.ContentFormatType.ApplicationJson,
                Payload = "{\"test\": 1234}"
            };
        }
    }
}
