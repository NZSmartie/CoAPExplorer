using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CoAPExplorer.WPF.Converters
{
    public class UIElementVisibilityToUnsetValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var element = parameter as UIElement;
            if (element == null)
                throw new ArgumentException($"Invalid parameter [{(parameter?.GetType()?.ToString() ?? "null")}], expecting [{nameof(UIElement)}]", nameof(parameter));

            if(!element.IsVisible)
                return DependencyProperty.UnsetValue;

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}