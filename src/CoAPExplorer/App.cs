using System;
using System.Collections.Generic;
using System.Text;
using CoAPExplorer.Extensions;
using CoAPExplorer.Services;
using CoAPExplorer.ViewModels;
using CoAPNet.Udp;
using ReactiveUI;
using Splat;

namespace CoAPExplorer
{
    public class App
    {
        public IMutableDependencyResolver Services { get; }

        public App()
        {
            Services = Locator.CurrentMutable;

#if DEBUG
            // Debug logging
            Services.RegisterConstant<ILogger>(new MyDebugLogger { Level = LogLevel.Debug });
#endif

            // Register logger for all require generic uses of Microsoft.Extensions.Logging.ILogger<T>
            //services.RegisterLogger<OicService>()
            //    .RegisterLogger<OICNet.OicClient>()
            //    .RegisterLogger<OICNet.OicResourceDiscoverClient>();

            // App-wide services
            Services
                .RegisterLogger<CoapUdpTransportFactory>()
                .RegisterConstant(new Navigation());

        }
    }
}
