using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;

namespace CoAPExplorer.ViewModels
{
    public class RecentDevicesViewModel : ReactiveObject, IRoutableViewModel
    {
        public ReactiveCommand<Unit, NewDeviceViewModel> AddDeviceCommand;

        public string UrlPathSegment => "Recent Devices";

        public IScreen HostScreen { get; }

        public RecentDevicesViewModel(IScreen hostScreen)
        {
            HostScreen = hostScreen;

            AddDeviceCommand = ReactiveCommand.Create<Unit, NewDeviceViewModel>(_ =>
            {
                System.Diagnostics.Debug.WriteLine("Creating new device dialog thingy");
                return new NewDeviceViewModel(HostScreen);
            });
        }
    }
}
