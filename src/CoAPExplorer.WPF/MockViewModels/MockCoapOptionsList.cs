using CoAPNet;
using CoAPNet.Options;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoAPExplorer.WPF.MockViewModels
{
    public class MockCoapOptionsList
    {
        public ObservableCollection<CoapOption> Options { get; }
        = new ObservableCollection<CoapOption>
        {
            new UriHost("localhost"),
            new UriPort(1234),
            new UriPath("path"),
            new UriQuery("ct=0"),
            new ProxyUri(),
            new ProxyScheme(),
            new LocationPath(),
            new LocationQuery(),
            new ContentFormat(ContentFormatType.ApplicationJson),
            new Accept(ContentFormatType.ApplicationJson),
            new MaxAge(3600),
            new ETag(new byte[]{12,34,56,78 }),
            new Size1(1234),
            new Size2(1234),
            new IfMatch(),
            new IfNoneMatch(),
            new Block1(),
            new Block2(1,128,true),
        };
    }
}
