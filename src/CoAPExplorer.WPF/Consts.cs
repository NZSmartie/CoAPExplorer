using CoAPNet;
using CoAPNet.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoAPExplorer.WPF
{
    public class Consts
    {
        public static IReadOnlyList<Tuple<string, CoapMessageCode>> MessageCodes { get; } = new List<Tuple<string, CoapMessageCode>>
        {
            Tuple.Create( "EMPTY", CoapMessageCode.None ),
            Tuple.Create( "GET", CoapMessageCode.Get ),
            Tuple.Create( "POST", CoapMessageCode.Post),
            Tuple.Create( "PUT", CoapMessageCode.Put ),
            Tuple.Create( "DELETE", CoapMessageCode.Delete),
        };

        public static IReadOnlyList<Tuple<string, ContentFormatType>> ContentTypes { get; } = new List<Tuple<string, ContentFormatType>>
        (
            new[]
            {
                ContentFormatType.TextPlain,
                ContentFormatType.ApplicationLinkFormat,
                ContentFormatType.ApplicationXml,
                ContentFormatType.ApplicationOctetStream,
                ContentFormatType.ApplicationExi,
                ContentFormatType.ApplicationJson,
                ContentFormatType.ApplicationCbor
            }
            .Select(cf => Tuple.Create($"{cf.Value} - {cf.Name}", cf))
            .Prepend(Tuple.Create("(none)", default(ContentFormatType)))
        );
    }
}
