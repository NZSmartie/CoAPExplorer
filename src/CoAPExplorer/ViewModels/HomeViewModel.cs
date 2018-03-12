using System;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Collections.Generic;
using System.Text;

using ReactiveUI;
using CoAPExplorer.Services;
using Splat;
using System.Reactive;
using CoAPExplorer.Models;

namespace CoAPExplorer.ViewModels
{
    public class HomeViewModel : ReactiveObject, IScreen, IRoutableViewModel, ISupportsActivation, ISupportsNavigatation
    {
        public RoutingState Router { get; } = new RoutingState();

        private readonly Navigation _navigation;

        public INavigationViewModel Navigation => _navigation.ViewModel;

        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public ObservableAsPropertyHelper<string> _urlPathSegment;
        public string UrlPathSegment => _urlPathSegment.Value;

        public IScreen HostScreen { get; } 

        public HomeViewModel(IScreen hostScreen)
        {
            HostScreen = hostScreen;

            _navigation = Locator.Current.GetService<Navigation>();
            _navigation.HostScreen = HostScreen; // MasterDetail view
            _navigation.HomeView = this; // Well, this

            _urlPathSegment = ObservableAsPropertyHelper<string>.Default();

            // TODO: Set Favourites? or last viewed?
            Router.NavigateAndReset
                .Execute(new SearchViewModel(hostScreen))
                .Subscribe();

            this.WhenActivated((CompositeDisposable disposables) =>
            {
                _urlPathSegment = Router.CurrentViewModel
                      .Select(rvm => rvm?.UrlPathSegment)
                      .ToProperty(this, vm => vm.UrlPathSegment)
                      .DisposeWith(disposables);

                Router.CurrentViewModel
                      .Subscribe(rvm => Navigation.IsOpen = rvm == null)
                      .DisposeWith(disposables);
            });
        }
    }
}
