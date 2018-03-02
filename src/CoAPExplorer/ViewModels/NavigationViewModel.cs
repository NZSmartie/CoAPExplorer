using System;
using System.Collections.Generic;
using System.Text;
using CoAPExplorer.Models;
using ReactiveUI;

namespace CoAPExplorer.ViewModels
{
    public class NavigationViewModel : ReactiveObject, INavigationViewModel
    {
        private bool _isOpen = false;
        public bool IsOpen { get => _isOpen; set => this.RaiseAndSetIfChanged(ref _isOpen, value); }

        public ReactiveList<NavigationItem> NavigationItems { get; set; }

        public NavigationItem SelectedNavigationItem { get; set; }
    }
}
