using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;

using CoAPExplorer.Models;
using CoAPExplorer.Services;
using CoAPExplorer.Extensions;
using ReactiveUI;
using Splat;
using CoAPExplorer.Database;

namespace CoAPExplorer.ViewModels
{
    public class NewDeviceViewModel : ReactiveObject
    {
        private string _name;
        private string _address;
        private IScreen _hostScreen;
        private readonly CoapExplorerContext _dbContext;
        private EndpointType _selectedTransport;

        public string Name { get => _name; set => this.RaiseAndSetIfChanged(ref _name, value); }

        public string Address { get => _address; set => this.RaiseAndSetIfChanged(ref _address, value); }

        public ReactiveList<Tuple<EndpointType, string>> Transports { get; } = new ReactiveList<Tuple<EndpointType, string>>();

        public EndpointType SelectedTransport { get => _selectedTransport; set => this.RaiseAndSetIfChanged(ref _selectedTransport, value); }

        public ReactiveCommand<Unit, Unit> AddDeviceCommand { get; }

        public NewDeviceViewModel(IScreen hostScreen)
        {
            _hostScreen = hostScreen;

            _dbContext = Locator.Current.GetService<CoapExplorerContext>();

            Transports.AddRange(Enum.GetValues(typeof(EndpointType))
                                    .Cast<EndpointType>()
                                    .Where(e => e != EndpointType.None)
                                    .Select(e => Tuple.Create(e, e.GetDisplayValue())));

            AddDeviceCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                // TODO: User Input Validation
                // TODO: Can re add another devie with the same Endpoint address?

                var device = new Device
                {
                    Name = Name,
                    Address = Address,
                    Endpoint = CoapEndpointFactory.GetEndpoint(Address, SelectedTransport),
                    EndpointType = SelectedTransport
                };

                _hostScreen.Router.Navigate
                                  .Execute(new DeviceViewModel(device))
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
