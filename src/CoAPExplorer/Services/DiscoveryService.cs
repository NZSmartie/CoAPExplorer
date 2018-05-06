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
using CoAPNet.Options;

namespace CoAPExplorer.Services
{
    public class DiscoveryService : IDiscoveryService
    {
        private readonly CoapClient _coapClient;
        private readonly CoapExplorerContext _dbContext;
        private readonly CoapMessage _discoverRequest;

        private TimeSpan _timeout = TimeSpan.FromSeconds(30);

        public DiscoveryService()
        {
            _coapClient = new CoapClient(CoapEndpointFactory.GetEndpoint(new Uri("coap://0.0.0.0/")));
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

        public IObservable<Device> DiscoverDevices()
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

                            var address = recv.Endpoint.BaseUri;
                            var device = _dbContext.Devices.SingleOrDefault(d => d.Address == address);
                            if (device == null)
                            {
                                device = new Device
                                {
                                    Endpoint = recv.Endpoint,
                                    LastSeen = DateTime.Now,
                                    Address = recv.Endpoint.BaseUri,
                                    Name = "(unnamed)"
                                };
                                await _dbContext.Devices.AddAsync(device);
                            }

                            device.LastSeen = DateTime.Now;

                            var contentForamt = recv.Message.Options.Get<ContentFormat>();
                            if (recv.Message.Code.IsSuccess() && contentForamt != null && 
                                contentForamt.MediaType == ContentFormatType.ApplicationLinkFormat)
                            {
                                device.KnownResources.Clear();
                                foreach (var resource in GetResources(device, Encoding.UTF8.GetString(recv.Message.Payload)))
                                    device.KnownResources.Add(resource);
                            }

                            await _dbContext.SaveChangesAsync();

                            observer.OnNext(device);
                        }
                        observer.OnCompleted();
                    }
                    catch (OperationCanceledException)
                    {
                        observer.OnCompleted();
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                    }
                });
                return () => cts.Cancel();
            });
        }

        public IObservable<DeviceResource> DiscoverResources(Device device)
        {
            return Observable.Create<DeviceResource>(observer =>
            {
                var cts = new CancellationTokenSource();
                var discoverRequest = new Message
                {
                    Url = "/.well-known/core",
                    Code = CoapMessageCode.Get,
                    Token = new byte[8],
                };

                // Assign a random token
                new Random().NextBytes(discoverRequest.Token);

                var coapService = new CoapService(device);

                coapService
                    .SendMessage(discoverRequest, device.Endpoint)
                    .Subscribe(message =>
                    {
                        if (message.Code == CoapMessageCode.NotFound)
                            return;

                        if (!message.Code.IsSuccess())
                        {
                            observer.OnError(new CoapMessageFormatException(message.Payload != null ? Encoding.UTF8.GetString(message.Payload) : message.Code.ToString(), message.Code));
                            return;
                        }

                        if (message.ContentFormat.Value != CoAPNet.Options.ContentFormatType.ApplicationLinkFormat.Value)
                        {
                            observer.OnError(new CoapMessageFormatException("Message is not applicaiton/link-format", message.Code));
                            return;
                        }

                        Observable.ToObservable(GetResources(device, Encoding.UTF8.GetString(message.Payload))).Subscribe(observer);

                    }, observer.OnError, observer.OnCompleted, cts.Token);

                return () => cts.Cancel();
            });
        }

        private IEnumerable<DeviceResource> GetResources(Device device, string payload)
        {
            var coreResources = CoreLinkFormat.Parse(payload);

            foreach (var coreResource in coreResources)
            {
                var resource = new DeviceResource
                {
                    Device = device,
                    Url = coreResource.UriReference.OriginalString,
                };

                if (!string.IsNullOrEmpty(coreResource.Title))
                    resource.Name = coreResource.Title;

                if (coreResource.SuggestedContentTypes.Count > 0)
                    resource.ContentFormat = coreResource.SuggestedContentTypes[0];

                yield return resource;
            }
        }
    }
}
