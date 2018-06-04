using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using CoAPExplorer.Extensions;
using CoAPExplorer.Models;
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

        private ISubject<ToastNotification> _toastNotifications
            = new Subject<ToastNotification>();

        public IObservable<ToastNotification> ToastNotifications => _toastNotifications;

        /// <summary>
        /// Logs the exception to the applications log directory and invokes the exception event for displaying to the user.
        /// </summary>
        /// <param name="exception"></param>
        public static void LogException(Exception exception)
        {
            if (exception == null)
                return;

            var app = Splat.Locator.Current.GetService<App>();

            // TODO: Fire an event which will display a toast of the recently received exception.
            // TODO: Create a view (seperate window?) for viewing all exceptions

            var logPath = new DirectoryInfo(Path.Combine(app.DataPath, "logs"));

            if (!logPath.Exists)
                logPath.Create();

            var timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH-mm-ss", System.Globalization.CultureInfo.InvariantCulture);
            var baseFileName = $"{timestamp}-Error-{exception.GetType().Name}";
            
            var attempt = 1;
            var filename = Path.Combine(app.DataPath, "logs", $"{baseFileName}.log");
            while(File.Exists(filename))
                filename = Path.Combine(app.DataPath, "logs", $"{baseFileName}{attempt++}.log");

            using (var log = new StreamWriter(filename, false, Encoding.UTF8))
                log.Write(exception.ToString());

            app._toastNotifications.OnNext(new ToastNotification($"{exception.GetType().Name}: {exception.Message}.", ToastNotificationType.Information,
                                                                 ("Show", OpenLogFile(filename))));
        }

        private static ReactiveCommand OpenLogFile(string filename)
        {
            return ReactiveCommand.Create(() =>
            {
                var process = Process.Start(filename);
            });
        }

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

            Locator.RegisterConstant(this);
            Locator.RegisterConstant<IReactiveApp>(this);
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
