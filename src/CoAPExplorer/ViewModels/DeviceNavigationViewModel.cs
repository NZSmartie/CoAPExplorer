using CoAPExplorer.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Text;

namespace CoAPExplorer.ViewModels
{
    public class DeviceNavigationViewModel : ReactiveObject, INavigationViewModel
    {
        private bool _isOpen = true;
        public bool IsOpen { get => _isOpen; set => this.RaiseAndSetIfChanged(ref _isOpen, value); }

        private DeviceViewModel _device;

        public string Name => _device.Name;

        public string Address => _device.Address;

        public DeviceNavigationViewModel(DeviceViewModel device)
        {
            _device = device;

            _device.PropertyChanged += DevicePropertyChanged;
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
