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

namespace CoAPExplorer.ViewModels
{
    public class SearchViewModel : ReactiveObject, IRoutableViewModel, ISupportsActivation
    {
        public string UrlPathSegment => "Device Search";

        public IScreen HostScreen { get; }

        public ObservableCollection<Device> Devices { get; }

        public List<RequestFilter> Filters { get; set; }

        public ReactiveCommand AddFilter { get; }
        public ReactiveCommand<RequestFilter,Unit> RemoveFilter { get; }

        public ReactiveCommand<Unit, Device> SearchCommand{ get; }
        public ReactiveCommand<Unit,Unit> StopCommand { get; }

        private string _searchUrl = "/.well-known/core";

        public string SearchUrl
        {
            get { return _searchUrl; }
            set { this.RaiseAndSetIfChanged(ref _searchUrl, value); }
        }

        ObservableAsPropertyHelper<bool> _isSearching;
        public bool IsSearching => _isSearching.Value;

        protected readonly ViewModelActivator viewModelActivator = new ViewModelActivator();
        public ViewModelActivator Activator => viewModelActivator; 

        private readonly DiscoveryService _discoveryService;

        public SearchViewModel(IScreen hostScreen)
        {
            HostScreen = hostScreen ?? throw new ArgumentNullException(nameof(hostScreen));

            _discoveryService = Locator.Current.GetService<DiscoveryService>();

            Devices = new ObservableCollection<Device>();

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

                SearchCommand.Subscribe(device => Devices.Add(device))
                             .DisposeWith(disposables);
            });
        }

        public IObservable<Device> SearchDevices()
        {
            _discoveryService.SetRequstUrl(SearchUrl);
            _discoveryService.SetFilters(Filters);
            return _discoveryService.Discover();
        }
    }
}