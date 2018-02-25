using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CoAPExplorer.Extensions;
using CoAPExplorer.Services;
using CoAPExplorer.ViewModels;
using CoAPExplorer.WPF.Views;
using CoAPNet;
using CoAPNet.Udp;
using ReactiveUI;
using Splat;

namespace CoAPExplorer.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly CoAPExplorer.App _coapExplorer;

        public static CoAPExplorer.App CoapExplorer => (Current as App)._coapExplorer;

        public App()
        {
            // Shared application class that is used in other platforms.
            _coapExplorer = new CoAPExplorer.App();

            _coapExplorer.Services
                .Register<IViewFor<SearchViewModel>>(() => new SearchView());

        }
    }
}
