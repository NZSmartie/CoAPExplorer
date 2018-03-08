using CoAPExplorer.Models;
using CoAPExplorer.Services;
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
            : this(null)
        { }

        public DeviceViewModel(Device device)
            : base(device ?? new Device
            {
                Name = "Some Device",
                EndpointType = EndpointType.Udp,
                Endpoint = CoapEndpointFactory.GetLocalEndpoint(EndpointType.Udp),
                Address = "192.168.x.x",
                LastSeen = DateTime.Now,
                IsFavourite = false,
            })
        {
            Message = new Message
            {
                MessageId = 1234,
                Token = new byte[] { 0x01, 0x02, 0x03, 0x04 },

                Url = "/some/resource",
                Code = CoapMessageCode.Get,
                //Type = CoapMessageType.Confirmable,

                ContentFormat = CoAPNet.Options.ContentFormatType.ApplicationJson,
                Payload = Encoding.UTF8.GetBytes("{\r\n\t\"test\": 1234, \r\n\t\"emoji\": \"🦆\", \r\n\t\"zero\": \0\r\n}")
            };
        }
    }
}
