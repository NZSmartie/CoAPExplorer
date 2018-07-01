using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;

using CoAPNet;
using CoAPNet.Udp;
using ReactiveUI;
using Splat;

using CoAPExplorer.Extensions;
using CoAPExplorer.Models;
using CoAPExplorer.Services;
using CoAPExplorer.ViewModels;


namespace CoAPExplorer
{
    public class App
    {
        public string DataPath { get; }

        private ISubject<ToastNotification> _toastNotifications
            = new Subject<ToastNotification>();

        public IObservable<ToastNotification> ToastNotifications => _toastNotifications;

        public IMutableDependencyResolver Locator => global::Splat.Locator.CurrentMutable;

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
                filename = Path.Combine(app.DataPath, "logs", $"{baseFileName}.{attempt++}.log");

            using (var log = new StreamWriter(filename, false, Encoding.UTF8))
                log.Write(exception.ToString());

            var message = "An error has occured.";
#if DEBUG
            // Only display exception details in the UI for debug builds
            message = $"{exception.GetType().Name}: {exception.Message}.";
#endif
            app._toastNotifications.OnNext(new ToastNotification(message, ToastNotificationType.Error, ("Show", OpenLogFile(filename))));
        }

        private static ReactiveCommand OpenLogFile(string filename)
        {
            return ReactiveCommand.Create(() =>
            {
                var process = Process.Start(filename);
            });
        }

        public App(string dataPath)
        {
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
    }
}
