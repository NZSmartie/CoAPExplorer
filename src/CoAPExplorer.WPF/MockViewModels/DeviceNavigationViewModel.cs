using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoAPExplorer.WPF.MockViewModels
{
    public class DeviceNavigationViewModel : CoAPExplorer.ViewModels.DeviceNavigationViewModel
    {
        public DeviceNavigationViewModel()
            :base(new DeviceViewModel())
        { }
    }
}
