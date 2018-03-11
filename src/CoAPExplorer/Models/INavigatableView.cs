using CoAPExplorer.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoAPExplorer.Models
{
    public interface INavigationViewModel
    {
        bool IsOpen { get; set; }
    }

    public interface ISupportsNavigatation
    {
        INavigationViewModel Navigation { get; }

        string UrlPathSegment { get; }
    }
}
