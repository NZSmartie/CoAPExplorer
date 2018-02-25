using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace CoAPExplorer.WPF.Converters
{
    public class RelativeDateTimeConverter : IValueConverter
    {
        private readonly Dictionary<long, string> _thresholds = new Dictionary<long, string>();

        public RelativeDateTimeConverter()
        {
            const int minute = 60;
            const int hour = 60 * minute;
            const int day = 24 * hour;
            _thresholds.Add(60, "{0} seconds ago");
            _thresholds.Add(minute * 2, "a minute ago");
            _thresholds.Add(45 * minute, "{0} minutes ago");
            _thresholds.Add(120 * minute, "an hour ago");
            _thresholds.Add(day, "{0} hours ago");
            _thresholds.Add(day * 2, "yesterday");
            _thresholds.Add(day * 30, "{0} days ago");
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime relative)
            {
                if (relative == DateTime.MinValue)
                    return "never";

                long since = (DateTime.Now.Ticks - relative.Ticks) / 10000000;

                var threshold = _thresholds.FirstOrDefault(t => since < t.Key);
                if (string.IsNullOrEmpty(threshold.Value))
                    return relative.ToShortDateString();

                TimeSpan timeSpan = new TimeSpan((DateTime.Now.Ticks - relative.Ticks));

                return string.Format(threshold.Value,
                    timeSpan.Days > 365 ? timeSpan.Days / 365 : (timeSpan.Days > 0 ? timeSpan.Days : (timeSpan.Hours > 0 ? timeSpan.Hours : (timeSpan.Minutes > 0 ? timeSpan.Minutes : (timeSpan.Seconds > 0 ? timeSpan.Seconds : 0)))));

            }

#if DEBUG
            return $"Could not cast {value?.GetType()} to {nameof(DateTime)}";
#else
            return "";
#endif
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}