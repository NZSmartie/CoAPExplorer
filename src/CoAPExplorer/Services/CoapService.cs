using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CoAPNet;
using Splat;

using CoAPExplorer.Extensions;
using CoAPExplorer.Models;

namespace CoAPExplorer.Services
{
    public class CoapService
    {
        private readonly CoapClient _coapClient;
        private readonly ICoapEndpoint _endpoint;

        public CoapService(EndpointType type)
        {
            _endpoint = CoapEndpointFactory.GetLocalEndpoint(type);
            _coapClient = new CoapClient(_endpoint);
        }

        public IObservable<Message> SendMessage(Message message, ICoapEndpoint endpoint)
        {
            if (message is null)
                return Observable.Empty<Message>();

            var coapMessage = message.ToCoapMessage();

            return Observable.Create<Message>(observer =>
            {
                var cts = new CancellationTokenSource();
                Task.Run(async () =>
                {
                    try
                    {
                        var id = await _coapClient.SendAsync(coapMessage, endpoint, cts.Token);

                        var response = await _coapClient.GetResponseAsync(id, cts.Token);

                        observer.OnNext(response.ToMessage());

                        observer.OnCompleted();
                    }
                    catch(Exception ex)
                    {
                        observer.OnError(ex);
                    }
                });

                return cts.Cancel;
            });
        }
    }
}
