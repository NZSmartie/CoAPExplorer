using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CoAPExplorer.WPF.Controls
{
    public class NavigationDrawer : ListView
    {
        public NavigationDrawer()
        {
            SelectionMode = SelectionMode.Single;
        }
    }
}
