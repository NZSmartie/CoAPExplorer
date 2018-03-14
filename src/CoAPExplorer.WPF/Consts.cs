using CoAPExplorer.WPF.Converters;
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
        (
            new[]
            {
                Tuple.Create( "None",CoapMessageCode.None),
                Tuple.Create( "Proxying Not Supported",CoapMessageCode. ProxyingNotSupported),
                Tuple.Create( "Gateway Timeout",CoapMessageCode. GatewayTimeout),
                Tuple.Create( "Service Unavailable",CoapMessageCode. ServiceUnavailable),
                Tuple.Create( "Bad Gateway",CoapMessageCode. BadGateway),
                Tuple.Create( "Not Implemented",CoapMessageCode. NotImplemented),
                Tuple.Create( "Internal Server Error",CoapMessageCode. InternalServerError),
                Tuple.Create( "Unsupported Content Format",CoapMessageCode. UnsupportedContentFormat),
                Tuple.Create( "Precondition Failed",CoapMessageCode. PreconditionFailed),
                Tuple.Create( "Request Entity Incomplete",CoapMessageCode. RequestEntityIncomplete),
                Tuple.Create( "Not Acceptable",CoapMessageCode. NotAcceptable),
                Tuple.Create( "Method Not Allowed",CoapMessageCode. MethodNotAllowed),
                Tuple.Create( "Not Found",CoapMessageCode. NotFound),
                Tuple.Create( "Forbidden",CoapMessageCode. Forbidden),
                Tuple.Create( "Request Entity Too Large",CoapMessageCode. RequestEntityTooLarge),
                Tuple.Create( "Unauthorized",CoapMessageCode. Unauthorized),
                Tuple.Create( "Bad Option",CoapMessageCode. BadOption),
                Tuple.Create( "Post",CoapMessageCode. Post),
                Tuple.Create( "Put",CoapMessageCode. Put),
                Tuple.Create( "Delete",CoapMessageCode. Delete),
                Tuple.Create( "Created",CoapMessageCode. Created),
                Tuple.Create( "Deleted",CoapMessageCode. Deleted),
                Tuple.Create( "Get",CoapMessageCode. Get),
                Tuple.Create( "Changed",CoapMessageCode. Changed),
                Tuple.Create( "Content",CoapMessageCode. Content),
                Tuple.Create( "Continue",CoapMessageCode. Continue),
                Tuple.Create( "Bad Request",CoapMessageCode. BadRequest),
                Tuple.Create( "Valid",CoapMessageCode. Valid),
            }
            .Select(t => Tuple.Create($"{t.Item2} - {t.Item1.ToUpper()}", t.Item2))
        );

        private static IReadOnlyList<Tuple<string, CoapMessageCode>> _requestMessageCodes;
        public static IReadOnlyList<Tuple<string, CoapMessageCode>> RequestMessageCodes 
            => _requestMessageCodes ?? (_requestMessageCodes = MessageCodes.Where(t => t.Item2.IsRequest()).OrderBy(t => t.Item2.Detail).ToList());

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

        public static IReadOnlyList<Tuple<string, Type>> CoapOptionTypes { get; } = new List<Tuple<string, Type>>
        {
                Tuple.Create("URI Host",typeof(UriHost)),
                Tuple.Create("URI Port",typeof(UriPort)),
                Tuple.Create("URI Path",typeof(UriPath)),
                Tuple.Create("URI Query",typeof(UriQuery)),
                Tuple.Create("Proxy URI",typeof(ProxyUri)),
                Tuple.Create("Proxy Scheme",typeof(ProxyScheme)),
                Tuple.Create("Location Path",typeof(LocationPath)),
                Tuple.Create("Location Query",typeof(LocationQuery)),
                Tuple.Create("Content Format",typeof(ContentFormat)),
                Tuple.Create("Accept",typeof(Accept)),
                Tuple.Create("Max Age",typeof(MaxAge)),
                Tuple.Create("ETag",typeof(ETag)),
                Tuple.Create("Size1",typeof(Size1)),
                Tuple.Create("Size2",typeof(Size2)),
                Tuple.Create("If Match",typeof(IfMatch)),
                Tuple.Create("If None Match",typeof(IfNoneMatch)),
                Tuple.Create("Block1",typeof(Block1)),
                Tuple.Create("Block2",typeof(Block2)),
        };

        public static IReadOnlyList<int> CoapBlockSupportedSizes { get; } = CoAPNet.Options.BlockBase.SupportedBlockSizes;
    }
}
