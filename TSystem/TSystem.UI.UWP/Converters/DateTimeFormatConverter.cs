using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.Web.Syndication;

namespace TSystem.UI.UWP.Converters
{
    public class DateTimeFormatConverter : IValueConverter
    {
        public string ConvertTo { get; set; }
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string format = value.ToString();
            DateTime date = (DateTime)value;
            if (date == null) return format;

            switch (ConvertTo.ToLower())
            {
                case "date":
                    format = date.ToShortDateString();
                    break;
                case "time":
                    format = date.ToShortTimeString();
                    break;
                default: break;
            }

            return format;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
