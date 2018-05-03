using CoAPExplorer.Database;
using CoAPExplorer.Models;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using ReactiveUI.Routing;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;

namespace CoAPExplorer.ViewModels
{
    public class RecentDevicesViewModel : ReactiveObject
    {
        private readonly CoapExplorerContext _dbContext;
        private readonly IReactiveRouter _router;
        private ObservableCollection<DeviceViewModel> _devices;

        public ReactiveCommand<Unit, NewDeviceViewModel> AddDeviceCommand;

        public ObservableCollection<DeviceViewModel> Devices
        {
            get => _devices ?? (_devices = new ObservableCollection<DeviceViewModel>(_dbContext.Devices.Include(x => x.KnownResources).Select(d => new DeviceViewModel(d, _router))));
            set => _devices = value;
        }

        public RecentDevicesViewModel(IReactiveRouter router = null, CoapExplorerContext context = null)
        {
            _router = router ?? Locator.Current.GetService<IReactiveRouter>();
            _dbContext = context ?? Locator.Current.GetService<CoapExplorerContext>();

            AddDeviceCommand = ReactiveCommand.Create<Unit, NewDeviceViewModel>(_ =>
            {
                System.Diagnostics.Debug.WriteLine("Creating new device dialog thingy");
                return new NewDeviceViewModel(router);
            });

        }
    }
}
