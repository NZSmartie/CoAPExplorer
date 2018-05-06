using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using CoAPExplorer.Extensions;
using CoAPExplorer.Services;
using CoAPExplorer.ViewModels;
using CoAPNet;
using CoAPNet.Udp;
using ReactiveUI;
using ReactiveUI.Routing;
using ReactiveUI.Routing.Presentation;
using Splat;

namespace CoAPExplorer
{
    public class App : IReactiveApp
    {
        private readonly IReactiveApp _reactiveApplication;

        public string DataPath { get; }

        public App(IReactiveApp reactiveApplication, string dataPath)
        {
            _reactiveApplication = reactiveApplication;

            // TODO: Make use of this later, for now disable it
            _reactiveApplication.SuspensionDriver.InvalidateState();

            DataPath = dataPath;

#if DEBUG
            // Debug logging
            Locator.RegisterConstant<ILogger>(new MyDebugLogger { Level = LogLevel.Debug });
#endif

            // Register logger for all require generic uses of Microsoft.Extensions.Logging.ILogger<T>
            //services.RegisterLogger<OicService>()
            //    .RegisterLogger<OICNet.OicClient>()
            //    .RegisterLogger<OICNet.OicResourceDiscoverClient>();

            // Ensure coap related schemas are supported
            CoapStyleUriParser.Register();

            // App-wide services
            Locator
                .RegisterLogger<CoapUdpTransportFactory>();

            Locator.Register<IDiscoveryService>(() => new DiscoveryService());


        }

        #region IReactiveApp Proxy Members

        public IReactiveRouter Router => _reactiveApplication.Router;

        public IAppPresenter Presenter => _reactiveApplication.Presenter;

        public ISuspensionHost SuspensionHost => _reactiveApplication.SuspensionHost;

        public ISuspensionDriver SuspensionDriver => _reactiveApplication.SuspensionDriver;

        public IMutableDependencyResolver Locator => _reactiveApplication.Locator;

        public ReactiveAppState BuildAppState()
        {
            return _reactiveApplication.BuildAppState();
        }

        public IObservable<Unit> LoadState(ReactiveAppState state)
        {
            return _reactiveApplication.LoadState(state);
        }

        public void RegisterDisposable(IDisposable disposable)
        {
            _reactiveApplication.RegisterDisposable(disposable);
        }

        public void Dispose()
        {
            _reactiveApplication.Dispose();
        }

        #endregion
    }
}
