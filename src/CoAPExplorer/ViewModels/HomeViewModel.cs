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
    public class HomeViewModel : ReactiveObject, IScreen, ISupportsActivation
    {
        private ObservableAsPropertyHelper<string> _pageTitle;
        public string PageTitle => _pageTitle.Value;

        private bool _isNavigationFocused = false;

        public bool IsNavigationFocused {
            get => _isNavigationFocused;
            set => this.RaiseAndSetIfChanged(ref _isNavigationFocused, value);
        }

        public RoutingState Router { get; }

        private readonly Navigation _navigation;
        public NavigationViewModel Navigation => _navigation.ViewModel;

        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public HomeViewModel()
        {
            Router = new RoutingState();

            _navigation = Locator.Current.GetService<Navigation>();
            _navigation.HostScreen = this;
            
            // TODO: Set Favourites? or last viewed?
            Router.NavigateAndReset
                .Execute(new SearchViewModel(this))
                .Subscribe();

            this.WhenActivated((CompositeDisposable disposables) =>
            {
                _pageTitle = Router.CurrentViewModel
                      .Select(rvm => rvm.UrlPathSegment)
                      .ToProperty(this, vm => vm.PageTitle, "")
                      .DisposeWith(disposables);
            });
        }
    }
}
