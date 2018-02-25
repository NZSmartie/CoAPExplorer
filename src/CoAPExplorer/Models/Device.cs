using System;

namespace CoAPExplorer.Models
{
    public class Device
    {
        public string Name { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public DateTime LastSeen { get; set; } = DateTime.MinValue;
    }
}