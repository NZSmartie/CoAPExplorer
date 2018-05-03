using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;

using CoAPNet;
using ReactiveUI;
using Splat;

using CoAPExplorer.Models;
using CoAPExplorer.Services;
using ReactiveUI.Routing;

namespace CoAPExplorer.ViewModels
{
    public class SearchViewModel : ReactiveObject, ISupportsActivation
    {
        private string _searchUrl = "/.well-known/core";
        private ObservableAsPropertyHelper<bool> _isSearching;
        private readonly IDiscoveryService _discoveryService;
        private ObservableCollection<DeviceViewModel> _devices;

        public IReactiveRouter Router { get; }

        public List<RequestFilter> Filters { get; set; }

        public ReactiveCommand AddFilter { get; }

        public ReactiveCommand<RequestFilter, Unit> RemoveFilter { get; }

        public ReactiveCommand<Unit, Device> SearchCommand { get; }

        public ReactiveCommand<Unit, Unit> StopCommand { get; }

        public bool IsSearching => _isSearching.Value;

        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public ObservableCollection<DeviceViewModel> Devices
        {
            get => _devices ?? (_devices = new ObservableCollection<DeviceViewModel>());
            set => _devices = value;
        }

        public string SearchUrl
        {
            get { return _searchUrl; }
            set { this.RaiseAndSetIfChanged(ref _searchUrl, value); }
        }

        public SearchViewModel(IReactiveRouter router = null)
        {
            Router = router ?? Locator.Current.GetService<IReactiveRouter>();

            _discoveryService = Locator.Current.GetService<IDiscoveryService>();

            Filters = new List<RequestFilter>();

            AddFilter = ReactiveCommand.Create(() => Filters.Add(new RequestFilter()));
            RemoveFilter = ReactiveCommand.Create<RequestFilter>(f => Filters.Remove(f));

            SearchCommand = ReactiveCommand
                .CreateFromObservable(
                    () => SearchDevices().TakeUntil(StopCommand));

            StopCommand = ReactiveCommand.Create(
                () => { },
                SearchCommand.IsExecuting);

            this.WhenActivated(disposables =>
            {
                _isSearching = SearchCommand.IsExecuting
                                            .Select(x => x)
                                            .ToProperty(this, x => x.IsSearching, false)
                                            .DisposeWith(disposables);

                SearchCommand.Select(d => new DeviceViewModel(d, router)).Subscribe(device => Devices.Add(device))
                             .DisposeWith(disposables);
            });
        }

        public IObservable<Device> SearchDevices()
        {
            _discoveryService.SetRequstUrl(SearchUrl);
            _discoveryService.SetFilters(Filters);
            return _discoveryService.DiscoverDevices();
        }
    }
}