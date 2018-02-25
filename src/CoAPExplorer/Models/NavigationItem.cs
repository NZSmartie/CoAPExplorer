using ReactiveUI;

namespace CoAPExplorer.Models
{
    public class NavigationItem
    {
        public string Name { get; set; }

        public ReactiveCommand Command { get; set; }

        public CoapExplorerIcon Icon { get; set; }
    }
}