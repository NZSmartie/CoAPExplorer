using System.ComponentModel.DataAnnotations;

namespace CoAPExplorer
{
    public enum EndpointType
    {
        [Display(Name = "None")]
        None,
        [Display(Name = "UDP")]
        Udp
    }
}