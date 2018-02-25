using System;
using System.Collections.Generic;
using System.Text;
using CoAPExplorer.Models;
using ReactiveUI;

namespace CoAPExplorer.ViewModels
{
    public class NavigationViewModel : ReactiveObject
    {
        public ReactiveList<NavigationItem> NavigationItems { get; set; }

        public NavigationItem SelectedNavigationItem { get; set; }
    }
}
