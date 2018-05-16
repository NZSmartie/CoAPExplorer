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
                Endpoint = CoapEndpointFactory.GetEndpoint(new Uri("coap://182.168.x.x/")),
                Address = new Uri("coap://182.168.x.x/"),
                LastSeen = DateTime.Now,
                IsFavourite = false,
                KnownResources = new System.Collections.ObjectModel.Collection<DeviceResource>
                {
                    new DeviceResource {Url = new Uri("/.well-known/core", UriKind.RelativeOrAbsolute)},
                    new DeviceResource {Url = new Uri("/doors", UriKind.RelativeOrAbsolute)},
                    new DeviceResource {Url = new Uri("/doors/garage", UriKind.RelativeOrAbsolute)},
                    new DeviceResource {Url = new Uri("/doors/entrance", UriKind.RelativeOrAbsolute)},
                    new DeviceResource {Url = new Uri("/lights", UriKind.RelativeOrAbsolute)},
                    new DeviceResource {Url = new Uri("/lights/desklamp", UriKind.RelativeOrAbsolute)},
                    new DeviceResource {Url = new Uri("/lights/lounge1", UriKind.RelativeOrAbsolute)},
                    new DeviceResource {Url = new Uri("/lights/lounge2", UriKind.RelativeOrAbsolute)},
                    new DeviceResource {Url = new Uri("/lights/lounge3", UriKind.RelativeOrAbsolute)},
                    new DeviceResource {Url = new Uri("/lights/lounge4", UriKind.RelativeOrAbsolute)},
                    new DeviceResource {Url = new Uri("/lights/lounge4", UriKind.RelativeOrAbsolute)},
                },
            })
        {
            Message = new Message
            {
                MessageId = 1234,
                Token = new byte[] { 0x01, 0x02, 0x03, 0x04 },

                Url = new Uri("/some/resource", UriKind.RelativeOrAbsolute),
                Code = CoapMessageCode.Get,
                //Type = CoapMessageType.Confirmable,

                ContentFormat = CoAPNet.Options.ContentFormatType.ApplicationJson,
                Payload = Encoding.UTF8.GetBytes("{\r\n\t\"test\": 1234, \r\n\t\"emoji\": \"🦆\", \r\n\t\"zero\": \0\r\n}")
            };
        }
    }
}
