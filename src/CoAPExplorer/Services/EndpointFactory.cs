using CoAPNet;
using CoAPNet.Udp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace CoAPExplorer.Services
{
    public class CoapEndpointFactory
    {
        public static ICoapEndpoint GetLocalEndpoint()
        {
            return new CoapUdpEndPoint();
        }

        //public static ICoapEndpoint GetMulticastEndpoint()
        //{
        //    var endpoint = new IPEndPoint(Coap.MulticastIPv4)
        //}
    }
}
