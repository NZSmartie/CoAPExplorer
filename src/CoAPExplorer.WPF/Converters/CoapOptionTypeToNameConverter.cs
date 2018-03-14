using CoAPNet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CoAPExplorer.WPF.Converters
{
    public class CoapOptionTypeToNameConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => Convert(value as CoapOption);

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => ConvertBack(value as Type);

        public static string Convert(CoapOption option)
        {
            if (option is null)
                return string.Empty;

            var name = Consts.CoapOptionTypes.SingleOrDefault(p => p.Item2 == option.GetType()).Item1
                ?? option.GetType().Name;
            
            return name;
        }

        public static CoapOption ConvertBack(Type optionType)
        {
            if (optionType is null)
                return null;

            return Activator.CreateInstance(optionType) as CoapOption;
        }
    }
}
