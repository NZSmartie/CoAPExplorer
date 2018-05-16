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
using ReactiveUI.Routing;

using CoAPExplorer.Database;
using CoAPExplorer.Models;
using CoAPExplorer.Services;

namespace CoAPExplorer.ViewModels
{
    public class RecentDevicesViewModel : ReactiveObject, ISupportsActivation
    {
        private readonly CoapExplorerContext _dbContext;
        private readonly IReactiveRouter _router;
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
            get => _devices ?? (_devices = new ObservableCollection<DeviceViewModel>(_dbContext.Devices.Include(x => x.KnownResources).Select(d => new DeviceViewModel(d, _router))));
            set => _devices = value;
        }

        public IReactiveDerivedList<DeviceViewModel> FilteredDevices => _filteredDevices ?? (_filteredDevices = Devices.CreateDerivedCollection(x => x, FilterDevicesBySearhTerms, signalReset: this.WhenAnyValue(vm => vm.SearchTerms)));

        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public RecentDevicesViewModel(IReactiveRouter router = null, CoapExplorerContext context = null)
        {
            _router = router ?? Locator.Current.GetService<IReactiveRouter>();
            _dbContext = context ?? Locator.Current.GetService<CoapExplorerContext>();

            AddDeviceCommand = ReactiveCommand.Create<Unit, NewDeviceViewModel>(_ =>
            {
                System.Diagnostics.Debug.WriteLine("Creating new device dialog thingy");
                return new NewDeviceViewModel(router);
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
                    return Observable.Empty<Unit>();

                var deviceViewModel = new DeviceViewModel(new Device { Address = uri }, router);

                return router.Navigate(NavigationRequest.Forward(deviceViewModel));
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
