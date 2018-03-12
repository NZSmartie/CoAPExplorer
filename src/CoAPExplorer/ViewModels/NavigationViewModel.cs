using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using CoAPExplorer.Models;
using ReactiveUI;

namespace CoAPExplorer.ViewModels
{
    public class NavigationViewModel : ReactiveObject, INavigationViewModel
    {
        private bool _isOpen = false;
        private NavigationItem _selectedNavigationItem;

        public bool IsOpen { get => _isOpen; set => this.RaiseAndSetIfChanged(ref _isOpen, value); }

        public ReactiveList<NavigationItem> NavigationItems { get; set; }

        public NavigationItem SelectedNavigationItem
        {
            get => _selectedNavigationItem;
            set
            {
                //if (_selectedNavigationItem == value)
                //    return;
                _selectedNavigationItem = null;

                if(value?.Command != null)
                    Observable.Return(Unit.Default).InvokeCommand(value.Command);

                this.RaisePropertyChanged(nameof(SelectedNavigationItem));
            }
        }
    }
}
