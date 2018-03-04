using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;

using CoAPExplorer.Models;
using CoAPExplorer.Services;
using CoAPExplorer.Extensions;
using ReactiveUI;

namespace CoAPExplorer.ViewModels
{
    public class NewDeviceViewModel : ReactiveObject
    {
        private string _name;
        private string _address;
        private IScreen _hostScreen;
        private EndpointType _selectedTransport;
        private readonly CoapEndpointFactory _endpointFactory;

        public string Name { get => _name; set => this.RaiseAndSetIfChanged(ref _name, value); }

        public string Address { get => _address; set => this.RaiseAndSetIfChanged(ref _address, value); }

        public ReactiveList<Tuple<EndpointType, string>> Transports { get; } = new ReactiveList<Tuple<EndpointType, string>>();

        public EndpointType SelectedTransport { get => _selectedTransport; set => this.RaiseAndSetIfChanged(ref _selectedTransport, value); }

        public ReactiveCommand<Unit, Unit> AddDeviceCommand { get; }

        public NewDeviceViewModel(IScreen hostScreen)
        {
            _hostScreen = hostScreen;

            _endpointFactory = new CoapEndpointFactory();

            Transports.AddRange(Enum.GetValues(typeof(EndpointType))
                                    .Cast<EndpointType>()
                                    .Where(e => e != EndpointType.None)
                                    .Select(e => Tuple.Create(e, e.GetDisplayValue())));

            AddDeviceCommand = ReactiveCommand.Create(() =>
            {
                var deviceViewModel = new DeviceViewModel(
                    new Device
                    {
                        Name = Name,
                        Address = Address,
                        Endpoint = _endpointFactory.GetEndpoint(Address, SelectedTransport),
                    });
                _hostScreen.Router.Navigate
                                  .Execute(deviceViewModel)
                                  .Subscribe();
            });
        }
    }
}
