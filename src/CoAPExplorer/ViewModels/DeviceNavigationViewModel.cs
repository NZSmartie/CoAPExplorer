using CoAPExplorer.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;

namespace CoAPExplorer.ViewModels
{
    public class DeviceNavigationViewModel : ReactiveObject, INavigationViewModel, ISupportsActivation
    {
        private bool _isOpen = true;
        public bool IsOpen { get => _isOpen; set => this.RaiseAndSetIfChanged(ref _isOpen, value); }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        private DeviceViewModel _device;

        private ObservableAsPropertyHelper<string> _name
            = ObservableAsPropertyHelper<string>.Default();
        public string Name => _name.Value;

        private ObservableAsPropertyHelper<string> _address
            = ObservableAsPropertyHelper<string>.Default();
        public string Address => _address.Value;

        public DeviceNavigationViewModel(DeviceViewModel device)
        {
            _device = device;

            this.WhenActivated(disposables =>
            {
                _name = _device.WhenAnyValue(d => d.Name)
                               .ToProperty(this, vm => vm.Name)
                               .DisposeWith(disposables);

                _address = _device.WhenAnyValue(d => d.Address)
                               .ToProperty(this, vm => vm.Address)
                               .DisposeWith(disposables);
            });
        }
    }
}
