using CoAPNet;
using System;
using System.Threading.Tasks;

namespace CoAPExplorer.Models
{
    public class Device
    {
        public int Id { get; set; }

        public ICoapEndpoint Endpoint { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public DateTime LastSeen { get; set; } = DateTime.MinValue;
    }
}