using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace TSystem.UI.UWP.Converters
{
    public class NumberRoundConverter : IValueConverter
    {
        public int DecimalPlaces { get; set; } = 2;
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string format = "{0:0.";
            for (int i = 0; i < DecimalPlaces; i++)
            {
                format += "0";
            }
            format += "}";
            return String.Format(format, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
