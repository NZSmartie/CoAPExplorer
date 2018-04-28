using CoAPExplorer.Database;
using CoAPExplorer.Models;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;

namespace CoAPExplorer.ViewModels
{
    public class RecentDevicesViewModel : ReactiveObject, IRoutableViewModel
    {
        private readonly CoapExplorerContext _dbContext;

        public ReactiveCommand<Unit, NewDeviceViewModel> AddDeviceCommand;

        public ObservableCollection<DeviceViewModel> Devices { get; }

        public string UrlPathSegment => "Recent Devices";

        public IScreen HostScreen { get; }

        public RecentDevicesViewModel(IScreen hostScreen = null)
        {
            _dbContext = Locator.Current.GetService<CoapExplorerContext>();

            HostScreen = hostScreen;
            
            AddDeviceCommand = ReactiveCommand.Create<Unit, NewDeviceViewModel>(_ =>
            {
                System.Diagnostics.Debug.WriteLine("Creating new device dialog thingy");
                return new NewDeviceViewModel(HostScreen);
            });

            var devices = _dbContext.Devices.Include(x => x.KnownResources).ToList();
            Devices = new ObservableCollection<DeviceViewModel>(devices.Select(d => new DeviceViewModel(d, HostScreen)));
        }
    }
}
