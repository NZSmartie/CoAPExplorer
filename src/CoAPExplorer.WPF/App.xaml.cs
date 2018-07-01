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
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using Splat;

using CoAPExplorer.Database;
using CoAPExplorer.Extensions;
using CoAPExplorer.Services;
using CoAPExplorer.ViewModels;
using CoAPExplorer.WPF.Views;
using CoAPExplorer.WPF.Dialogs;

namespace CoAPExplorer.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly CoAPExplorer.App _coapExplorer;
        private readonly CoapExplorerContext _database;

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
            _database = new CoapExplorerContext(databasePath);
            _database.Database.Migrate();

            // Register Services
            _coapExplorer.Locator.RegisterConstant(_database);

            // Register Views
            _coapExplorer.Locator.Register<IViewFor<HomeViewModel>>(() => new HomeView());
            _coapExplorer.Locator.Register<IViewFor<RecentDevicesViewModel>>(() => new RecentDevicesView());
            _coapExplorer.Locator.Register<IViewFor<SearchViewModel>>(() => new SearchView());
            _coapExplorer.Locator.Register<IViewFor<DeviceViewModel>>(() => new DeviceView());
            _coapExplorer.Locator.Register<IViewFor<NavigationViewModel>>(() => new NavigationView());
            _coapExplorer.Locator.Register<IViewFor<DeviceNavigationViewModel>>(() => new DeviceNavigationView());

            // Dialogs
            _coapExplorer.Locator.Register<IViewFor<NewDeviceViewModel>>(() => new NewDeviceViewDialog());

            //_coapExplorer.Services
            //    .RegisterConstant(new CoapContext(databasePath));
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _database.Dispose();
            base.OnExit(e);
        }
    }
}
