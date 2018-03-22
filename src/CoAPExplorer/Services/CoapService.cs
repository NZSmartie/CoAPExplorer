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
using System.IO;

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
            var messageContext = coapMessage.CreateBlockWiseContext(_coapClient);

            return Observable.Create<Message>(observer =>
            {
                var cts = new CancellationTokenSource();
                Task.Run(async () =>
                {
                    try
                    {
                        if (coapMessage.IsBlockWise())
                        {
                            using (var writer = new CoapBlockStreamWriter(messageContext, endpoint))
                                await message.PayloadStream.CopyToAsync(writer, writer.BlockSize);
                        }
                        else
                        {
                            var id = await _coapClient.SendAsync(coapMessage, endpoint, cts.Token);
                            messageContext = new CoapBlockWiseContext(_coapClient, coapMessage, await _coapClient.GetResponseAsync(id, cts.Token));
                        }

                        var response = messageContext.Response.ToMessage();

                        if (messageContext.Response.IsBlockWise())
                        {
                            var memoryStream = new MemoryStream();

                            using (var reader = new CoapBlockStreamReader(messageContext, endpoint))
                                reader.CopyTo(memoryStream);

                            response.Payload = memoryStream.ToArray();
                        }

                        observer.OnNext(response);

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
