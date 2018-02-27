using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using CoAPNet;
using CoAPNet.Udp;
using ReactiveUI;
using Splat;

using CoAPExplorer.Extensions;
using CoAPExplorer.Services;
using CoAPExplorer.ViewModels;
using CoAPExplorer.WPF.Views;

namespace CoAPExplorer.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly CoAPExplorer.App _coapExplorer;

        public static CoAPExplorer.App CoapExplorer => (Current as App)._coapExplorer;

        public const string DatabaseName = "database.db";

        public App()
        {
            // Inistalise our application's data directory
            var applicationPath = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CoAPExplorer"));
            if (!applicationPath.Exists)
                applicationPath.Create();

            // Shared application class that is used in other platforms.
            _coapExplorer = new CoAPExplorer.App(applicationPath.FullName);

            // TODO: Make this configurable? as to make this application portable?
            var databasePath = Path.Combine(applicationPath.FullName, DatabaseName);

            // Register our servies and Views
            _coapExplorer.Services
                .Register<IViewFor<SearchViewModel>>(() => new SearchView());

            //_coapExplorer.Services
            //    .RegisterConstant(new CoapContext(databasePath));
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }
    }
}
