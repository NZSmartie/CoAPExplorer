using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace CoAPExplorer.WPF.Extensions
{
    public static class DependencyObjectExtensions
    {
        public static DependencyObject GetParent(this DependencyObject element)
        {
            if (element == null)
                return null;

            DependencyObject parent = null;

            if (element is ContentElement ce)
            {
                parent = ContentOperations.GetParent(ce);
                if (parent == null && ce is FrameworkContentElement fce)
                    parent = fce.Parent;

            }else if (element is FrameworkElement fe)
            {
                parent = fe.Parent;
            }

            if (parent != null)
                return parent;
            
            return VisualTreeHelper.GetParent(element);
        }

        public static IEnumerable<DependencyObject> GetChildren(this DependencyObject element)
        {
            if (element == null)
                yield break;

            var count = VisualTreeHelper.GetChildrenCount(element);

            for (var i = 0; i < count; i++)
                yield return VisualTreeHelper.GetChild(element, i);
        }
    }
}
