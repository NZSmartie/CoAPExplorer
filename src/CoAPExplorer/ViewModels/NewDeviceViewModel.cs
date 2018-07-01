using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;

using ReactiveUI;
using Splat;

using CoAPExplorer.Models;
using CoAPExplorer.Services;
using CoAPExplorer.Extensions;
using CoAPExplorer.Database;

namespace CoAPExplorer.ViewModels
{
    public class NewDeviceViewModel : ReactiveObject
    {
        private string _name;
        private string _address;
        private readonly CoapExplorerContext _dbContext;
        private readonly IScreen _screen;

        public string Name { get => _name; set => this.RaiseAndSetIfChanged(ref _name, value); }

        public string Address { get => _address; set => this.RaiseAndSetIfChanged(ref _address, value); }

        public ReactiveCommand<Unit, Unit> AddDeviceCommand { get; }

        public NewDeviceViewModel(IScreen screen = null)
        {
            _dbContext = Locator.Current.GetService<CoapExplorerContext>();

            _screen = screen ?? Locator.Current.GetService<IScreen>();

            AddDeviceCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                // TODO: User Input Validation
                // TODO: Can re add another devie with the same Endpoint address?
                var address = CoapEndpointFactory.CreateUriFromAddress(Address);
                var device = new Device
                {
                    Name = Name,
                    Address = address,
                    Endpoint = CoapEndpointFactory.GetEndpoint(address),
                };

                _screen.Router.Navigate.Execute(new DeviceViewModel(device, _screen))
                          .Subscribe();

                if (_dbContext != null)
                {
                    await _dbContext.Devices.AddAsync(device);
                    await _dbContext.SaveChangesAsync();
                }
            });
        }
    }
}
