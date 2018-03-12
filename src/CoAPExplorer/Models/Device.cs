using CoAPExplorer.Services;
using CoAPNet;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace CoAPExplorer.Models
{
    public class Device
    {
        private ICoapEndpoint _endpoint;

        [Key]
        public int Id { get; set; }

        [NotMapped]
        public ICoapEndpoint Endpoint
        {
            get => _endpoint ?? (_endpoint = CoapEndpointFactory.GetEndpoint(Address, EndpointType));
            set => _endpoint = value;
        }

        public List<DeviceResource> KnownResources { get; set; }
         = new List<DeviceResource>();

        public EndpointType EndpointType { get; set; }

        public bool IsFavourite { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public DateTime LastSeen { get; set; } = DateTime.MinValue;
    }
}