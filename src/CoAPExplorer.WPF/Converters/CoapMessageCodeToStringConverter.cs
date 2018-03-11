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
    public class CoapMessageCodeToStringConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
         => Convert(value as CoapMessageCode);

        public string Convert(CoapMessageCode value)
        {
            return Consts.MessageCodes.SingleOrDefault(c => c.Item2 == value)?.Item1 ?? "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
