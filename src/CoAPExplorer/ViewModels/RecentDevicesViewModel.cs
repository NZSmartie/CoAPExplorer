using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;

using Microsoft.EntityFrameworkCore;
using Splat;
using ReactiveUI;

using CoAPExplorer.Database;
using CoAPExplorer.Models;
using CoAPExplorer.Services;

namespace CoAPExplorer.ViewModels
{
    public class RecentDevicesViewModel : ReactiveObject, ISupportsActivation
    {
        private readonly CoapExplorerContext _dbContext;
        private readonly IScreen _screen;
        private ObservableAsPropertyHelper<bool> _isSearchValidUri = ObservableAsPropertyHelper<bool>.Default();
        private string _searchTerms;
        private ObservableCollection<DeviceViewModel> _devices;
        private IReactiveDerivedList<DeviceViewModel> _filteredDevices;

        public ReactiveCommand<Device, Unit> RemoveDeviceCommand { get; }

        public ReactiveCommand NavigateToUriCommand { get; }

        public string SearchTerms { get => _searchTerms; set => this.RaiseAndSetIfChanged(ref _searchTerms, value); }

        public bool IsSearchValidUri => _isSearchValidUri.Value;

        public ReactiveCommand<Unit, NewDeviceViewModel> AddDeviceCommand { get; }

        public ObservableCollection<DeviceViewModel> Devices
        {
            get => _devices ?? (_devices = new ObservableCollection<DeviceViewModel>(_dbContext.Devices.Include(x => x.KnownResources).Select(d => new DeviceViewModel(d, _screen))));
            set => _devices = value;
        }

        public IReactiveDerivedList<DeviceViewModel> FilteredDevices => _filteredDevices ?? (_filteredDevices = Devices.CreateDerivedCollection(x => x, FilterDevicesBySearhTerms, signalReset: this.WhenAnyValue(vm => vm.SearchTerms)));

        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public RecentDevicesViewModel(IScreen screen = null, CoapExplorerContext context = null)
        {
            _screen = screen ?? Locator.Current.GetService<IScreen>();
            _dbContext = context ?? Locator.Current.GetService<CoapExplorerContext>();

            AddDeviceCommand = ReactiveCommand.Create<Unit, NewDeviceViewModel>(_ =>
            {
                System.Diagnostics.Debug.WriteLine("Creating new device dialog thingy");
                return new NewDeviceViewModel(screen);
            });

            RemoveDeviceCommand = ReactiveCommand.CreateFromTask<Device>(async device =>
            {
                _dbContext.Devices.Remove(device);
                _devices.Remove(_devices.Single(dvm => dvm.Device.Id == device.Id));
                await _dbContext.SaveChangesAsync();
            });

            NavigateToUriCommand = ReactiveCommand.CreateFromObservable(() =>
            {
                if (!Uri.TryCreate(SearchTerms, UriKind.Absolute, out var uri))
                    return Observable.Empty<IRoutableViewModel>();

                var deviceViewModel = new DeviceViewModel(new Device { Address = uri }, screen);

                return screen.Router.Navigate.Execute(deviceViewModel);
            }, this.WhenAnyValue(vm => vm.IsSearchValidUri));

            this.WhenActivated(disposables =>
            {
                _isSearchValidUri = this.WhenAnyValue(vm => vm.SearchTerms)
                                        .Select(x => Uri.TryCreate(x, UriKind.Absolute, out _))
                                        .ToProperty(this, vm => vm.IsSearchValidUri)
                                        .DisposeWith(disposables);
            });
        }

        private bool FilterDevicesBySearhTerms(DeviceViewModel device)
        {
            if (IsSearchValidUri)
                return false;

            if (string.IsNullOrEmpty(SearchTerms))
                return true;

            var search = SearchTerms.ToLowerInvariant();

            if (device.Name.ToLowerInvariant().Contains(search) ||
                device.Address.ToLowerInvariant().Contains(search))
                return true;

            // TODO: Add more places to search (i.e. hostname, history, resources)

            return false;
        }
    }
}
