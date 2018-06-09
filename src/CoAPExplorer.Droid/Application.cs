using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using ReactiveUI;
using Splat;

using CoAPExplorer;
using CoAPExplorer.Database;
using CoAPExplorer.ViewModels;
using CoAPExplorer.Droid.Activities;
using ReactiveUI.Routing;
using ReactiveUI.Routing.Android;

namespace CoAPExplorer.Droid
{
    [Application(AllowBackup = true, Theme = "@style/AppTheme", Label = "@string/app_name")]
    public class Application : Android.App.Application
    {
        private App _coapExplorer;
        private CoapExplorerContext _database;

        public static App CoapExplorer => (Context as Application)._coapExplorer;

        public const string DatabaseName = "database.db";

        /// <summary>
        /// Required constructor for an Android Application
        /// </summary>
        /// <param name="javaReference"></param>
        /// <param name="transfer"></param>
        public Application(IntPtr javaReference, JniHandleOwnership transfer) 
            : base(javaReference, transfer)
        { }

        public override void OnCreate()
        {
            base.OnCreate();

            var packageInfo = PackageManager.GetPackageInfo(PackageName, 0);

            // Inistalise our application's data directory
            var applicationPath = new DirectoryInfo(Path.Combine(packageInfo.ApplicationInfo.DataDir, "databases"));
            if (!applicationPath.Exists)
                applicationPath.Create();

            // Shared application class that is used in other platforms.
            _coapExplorer = new CoAPExplorer.App(
                new ReactiveAppBuilder()
                    .AddReactiveRouting()
                    .ConfigureAndroid(this)
                    .Build(), 
                applicationPath.FullName);

            // TODO: Make this configurable? as to make this application portable?
            var databasePath = Path.Combine(applicationPath.FullName, DatabaseName);
            _database = new CoapExplorerContext(databasePath);
            _database.Database.Migrate();

            // Register Services
            _coapExplorer.Locator.RegisterConstant(_database);

            // Register Views
            //_coapExplorer.Services.Register<IViewFor<HomeViewModel>>(() => new HomeView());
            //_coapExplorer.Services.Register<IViewFor<RecentDevicesViewModel>>(() => new RecentDevicesView());
            //_coapExplorer.Services.Register<IViewFor<SearchViewModel>>(() => new SearchView());
            _coapExplorer.Locator.Register<IViewFor<DeviceViewModel>>(() => new DeviceActivity());
            //_coapExplorer.Services.Register<IViewFor<NavigationViewModel>>(() => new NavigationView());
            //_coapExplorer.Services.Register<IViewFor<DeviceNavigationViewModel>>(() => new DeviceNavigationView());

            //// Dialogs
            //_coapExplorer.Services.Register<IViewFor<NewDeviceViewModel>>(() => new NewDeviceViewDialog());


            //_coapExplorer.Services
            //    .RegisterConstant(new CoapContext(databasePath));
        }
    }
}