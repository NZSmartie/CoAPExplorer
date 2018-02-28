using System;
using System.Globalization;
using System.Windows.Data;
using MaterialDesignThemes.Wpf;

namespace CoAPExplorer.WPF.Converters
{
    public class CoapExplorerIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(targetType != typeof(PackIconKind))
                throw new NotSupportedException();

            if (value is CoapExplorerIcon icon)
            {
                switch (icon)
                {
                    case CoapExplorerIcon.None:
                        return null;
                    case CoapExplorerIcon.Settings:
                        return PackIconKind.Settings;
                    case CoapExplorerIcon.Search:
                        return PackIconKind.Magnify;
                    case CoapExplorerIcon.Favouriate:
                        return PackIconKind.Star;
                    case CoapExplorerIcon.Recent:
                        return PackIconKind.History;
                    default:
#if DEBUG
                        return PackIconKind.EmoticonPoop;
#else
                        return null;               
#endif
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}