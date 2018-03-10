using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CoAPExplorer.WPF.Converters
{
    public class HextoAsciiConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => Convert((byte[])value, targetType, (int)parameter, culture);

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => ConvertBack((string) value, targetType, (int)parameter, culture);

        public string Convert(byte[] value, Type targetType, int maxBytes, CultureInfo culture)
        {
            if(!(value is IEnumerable<byte> bytes))
                return string.Empty;

            var sb = new StringBuilder();
            var length = Math.Min(maxBytes, value.Length);
            if (length == 0)
                return "";

            sb.Append(string.Join(" ", value.Take(length - 1).Select(b => b.ToString("X2"))));

            if (length > 1)
                sb.Append(" ");

            var last = value.ElementAt(length-1);
            if (last < 0x10)
                sb.Append(last.ToString("X"));
            else if (length < maxBytes)
                sb.Append(last.ToString("X2"))
                  .Append(" ");
            else
                sb.Append(last.ToString("X2"));


            return sb.ToString();
        }

        public byte[] ConvertBack(string value, Type targetType, int maxBytes, CultureInfo culture)
        {
            var hexChars = "abcdefABCDEF0123456789";

            var length = Math.Max(maxBytes, (value.Length + 1) / 2);
            var arr = new List<byte>();

            var hex = "";
            int count = 0;
            foreach (var c in value)
            {
                if (hexChars.Contains(c))
                    hex += c;
                
                
                if (hex.Length == 2 || !char.IsLetterOrDigit(c))
                {
                    if(!string.IsNullOrWhiteSpace(hex))
                        arr.Add(System.Convert.ToByte(hex,16));

                    if (maxBytes == arr.Count)
                        break;

                    hex = "";
                    count++;
                }
            }

            if (maxBytes != arr.Count && !string.IsNullOrWhiteSpace(hex))
                arr.Add(System.Convert.ToByte(hex, 16));

            return arr.ToArray();
        }

        private static int GetHexVal(char hex)
        {
            int val = (int)hex;
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }
    }
}
