using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ReactiveUI;

using CoAPExplorer.ViewModels;

namespace CoAPExplorer.WPF.MockViewModels
{
    public class MockHomeView : HomeViewModel
    {
        public MockHomeView() 
        {
            RecentDevices = new RecentDevicesViewModel
            {
                Devices = new DeviceListMock().Devices
            };

            Search = new SearchViewModel()
            {
                Devices = new ReactiveList<ViewModels.DeviceViewModel>(RecentDevices.Devices)
            };
        }
    }
}
