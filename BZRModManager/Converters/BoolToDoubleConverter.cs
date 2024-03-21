using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BZRModManager.Converters
{
    internal class BoolToDoubleConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            try
            {
                if (value is bool b && parameter is string p)
                {
                    string[] output = p.Split('|');

                    if (b)
                        return double.Parse(output[1]);
                    return double.Parse(output[0]);
                }
            }
            catch { }
            return 0d;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
