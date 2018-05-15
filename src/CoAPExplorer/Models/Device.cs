using CoAPExplorer.Services;
using CoAPNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace CoAPExplorer.Models
{
    public class Device
    {
        private ICoapEndpoint _endpoint;
        private Uri _address = null;

        [Key]
        public int Id { get; set; }

        [NotMapped]
        public ICoapEndpoint Endpoint
        {
            get => _endpoint ?? (_endpoint = CoapEndpointFactory.GetEndpoint(Address));
            set => _endpoint = value;
        }

        public ICollection<DeviceResource> KnownResources { get; set; }
            = new ObservableCollection<DeviceResource>();

        public EndpointType EndpointType { get; set; }

        public bool IsFavourite { get; set; }

        public string Name { get; set; } = string.Empty;

        [NotMapped]
        public Uri Address { get => _address ?? (_address = Endpoint.BaseUri); set => _address = value; }
        public DateTime LastSeen { get; set; } = DateTime.MinValue;

        [Column(nameof(Address))]
        public string _dbAddress
        {
            get => Address?.ToString();
            set
            {
                if (value == null)
                {
                    Address = null;
                    return;
                }

                Address = new Uri(value, UriKind.Absolute);
            }
        }
    }
}