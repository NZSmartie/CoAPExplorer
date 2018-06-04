using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;

using ReactiveUI;
using Splat;

using CoAPExplorer.Database;
using CoAPExplorer.Models;
using CoAPExplorer.Services;

namespace CoAPExplorer.ViewModels
{
    public class DeviceNavigationViewModel : ReactiveObject, ISupportsActivation
    {
        private bool _isOpen = true;
        private IDiscoveryService _discoveryService;
        private readonly CoapExplorerContext _context;
        private bool _pendingClearResources = false;
        private DeviceViewModel _device;
        public DeviceResource _selectedResource;

        public bool IsOpen { get => _isOpen; set => this.RaiseAndSetIfChanged(ref _isOpen, value); }

        public Device Device => _device.Device;

        public IReactiveDerivedList<DeviceResource> Resources => _device.Resources;

        public DeviceResource SelectedResource { get => _selectedResource; set => this.RaiseAndSetIfChanged(ref _selectedResource, value); }

        public string Name => _device.Name;

        public string Address => _device.Address;

        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public ReactiveCommand<Device, DeviceResource> RefreshResourcesCommand;

        public DeviceNavigationViewModel(DeviceViewModel device)
        {
            _device = device;
            _discoveryService = Locator.Current.GetService<IDiscoveryService>();
            _context = Locator.Current.GetService<Database.CoapExplorerContext>();

            _device.PropertyChanged += DevicePropertyChanged;

            RefreshResourcesCommand = ReactiveCommand.CreateFromObservable<Device, DeviceResource>(d =>
            {
                _pendingClearResources = true;
                return _discoveryService.DiscoverResources(d).Do(_ => { }, async () => await _context.SaveChangesAsync());
            });

            RefreshResourcesCommand.Subscribe(resource =>
            {
                if (_pendingClearResources)
                    _device.Device.KnownResources.Clear();

                _pendingClearResources = false;
                _device.Device.KnownResources.Add(resource);
            });

            this.WhenActivated(disposables =>
            {
                RefreshResourcesCommand
                    .ThrownExceptions
                    .Subscribe(ex => App.LogException(ex))
                    .DisposeWith(disposables);
            });
        }

        private void DevicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(_device.Name):
                    this.RaisePropertyChanged(nameof(Name));
                    break;
                case nameof(_device.Address):
                    this.RaisePropertyChanged(nameof(Address));
                    break;
            }
        }
    }
}
