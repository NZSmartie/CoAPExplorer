using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace CoAPExplorer.WPF.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public Visibility WhenTrue { get; set; } = Visibility.Visible;

        public Visibility WhenFalse { get; set; } = Visibility.Collapsed;

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isVisible)
                return Convert(isVisible);

            return null;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
                return ConvertBack(visibility);

            return false;
        }

        Visibility Convert(bool value)
        {
            return value ? WhenTrue : WhenFalse;
        }

        bool ConvertBack(Visibility value)
        {
            return value == WhenTrue ? true : false;
        }
    }
}
