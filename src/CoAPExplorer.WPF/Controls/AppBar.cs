using System.Windows;
using System.Windows.Controls;

namespace CoAPExplorer.WPF.Controls
{
    public class AppBar : ContentControl
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            nameof(Title), typeof(string), typeof(AppBar), new PropertyMetadata(string.Empty));

        public string Title { get => GetValue(TitleProperty) as string; set => SetValue(TitleProperty, value); }
    }
}
