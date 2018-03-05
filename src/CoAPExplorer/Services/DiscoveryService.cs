using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;

using CoAPNet;
using Splat;

using CoAPExplorer.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoAPExplorer.Database;

namespace CoAPExplorer.Services
{
    public class DiscoveryService
    {
        private readonly CoapClient _coapClient;
        private readonly CoapExplorerContext _dbContext;
        private readonly CoapMessage _discoverRequest;

        private TimeSpan _timeout = TimeSpan.FromSeconds(30);

        public DiscoveryService()
        {
            _coapClient = new CoapClient(CoapEndpointFactory.GetLocalEndpoint());
            _dbContext = Locator.Current.GetService<CoapExplorerContext>();

            _discoverRequest = new CoapMessage()
            {
                IsMulticast = true,
                Type = CoapMessageType.NonConfirmable,
                Code = CoapMessageCode.Get
            };
        }

        public void SetRequstUrl(string Url)
        {
            _discoverRequest.SetUri(Url, UriComponents.Path);
        }

        public void SetFilters(IEnumerable<RequestFilter> filters)
        {
            _discoverRequest.SetUri(string.Join("&", filters.Select(f => $"{f.Key}={f.Value}")), UriComponents.Query);
        }

        public void SetTimeout(TimeSpan timeout)
        {
            _timeout = timeout;
        }

        public IObservable<Device> Discover()
        {
            return Observable.Create<Device>(observer =>
            {
                var cts = new CancellationTokenSource(_timeout);
                Task.Run(async () =>
                {
                    try
                    {
                        await _coapClient.SendAsync(_discoverRequest);

                        while (!cts.IsCancellationRequested)
                        {
                            var recv = await _coapClient.ReceiveAsync(cts.Token);

                            if (recv.Endpoint == _coapClient.Endpoint)
                                continue;

                            var address = recv.Endpoint.ToString();
                            var device = _dbContext.Devices.SingleOrDefault(d => d.Address == address);
                            if (device == null)
                            {
                                device = new Device
                                {
                                    Endpoint = recv.Endpoint,
                                    EndpointType = EndpointType.Udp,
                                    LastSeen = DateTime.Now,
                                    Address = recv.Endpoint.ToString(),
                                    Name = "(unnamed)"
                                };
                                _dbContext.Devices.Add(device);
                            }

                            device.LastSeen = DateTime.Now;
                            _dbContext.SaveChanges();

                            observer.OnNext(device);
                        }
                        observer.OnCompleted();
                    }
                    catch (OperationCanceledException)
                    {
                        observer.OnCompleted();
                    }
                    catch(Exception ex)
                    {
                        observer.OnError(ex);
                    }
                });
                return () => cts.Cancel();
            });
        }
    }
}
