using System;
using System.Globalization;
using Kramer.Common.Extensions;
using Windows.UI.Xaml.Data;

namespace Kramer.Common
{
    public class DateTimeToDateStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var date = (DateTime)value;
            return date.ToSwedishDate();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}