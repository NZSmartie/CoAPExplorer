using CoAPExplorer.Models;
using CoAPNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace CoAPExplorer.Extensions
{
    internal static class CoapMessageExtensions
    {
        public static CoapMessage ToCoapMessage(this Message message)
        {
            var coapMessage = new CoapMessage
            {
                Id = message.MessageId,
                Token = message.Token ?? new byte[] { },
                Code = message.Code
            };
            coapMessage.SetUri(message.Url, UriComponents.PathAndQuery);

            if (message.Code.IsRequest())
                coapMessage.Options.Add(new CoAPNet.Options.ContentFormat(message.ContentFormat));

            return coapMessage;
        }

        public static Message ToMessage(this CoapMessage coapMessage)
        {
            var contentTypeOption 
                = coapMessage.Options.FirstOrDefault(o => o.OptionNumber == CoapRegisteredOptionNumber.ContentFormat) 
                    as CoAPNet.Options.ContentFormat;

            var message = new Message
            {
                MessageId = coapMessage.Id,
                Token = coapMessage.Token,
                Code = coapMessage.Code,

                ContentFormat = contentTypeOption?.MediaType,

                Url = coapMessage.GetUri().GetComponents(UriComponents.PathAndQuery, UriFormat.UriEscaped),
            };

            message.Options = new ObservableCollection<CoapOption>(
                coapMessage.Options.Where(o => o.OptionNumber != CoapRegisteredOptionNumber.ContentFormat));

            return message;
        }
    }
}
