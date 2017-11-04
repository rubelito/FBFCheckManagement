using System;
using System.Windows.Data;

namespace FBFCheckManagement.WPF.Converters
{
    public class DateToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string dateStr;
            var date = value as DateTime?;
            if (date == null)
                dateStr = null;
            else if (IsDateHasHoursAndMinute(date.Value))
            {
                dateStr = date.Value.ToString("MMM dd, yyyy (h:mm tt)");
            }
            else
            {
                dateStr = date.Value.ToString("MMM dd, yyyy");
            }
            return dateStr;
        }

        private bool IsDateHasHoursAndMinute(DateTime dateTime)
        {
            return dateTime.Hour > 0 && dateTime.Minute > 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}