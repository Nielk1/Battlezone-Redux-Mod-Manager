using Avalonia.Data.Converters;
using BZRModManager.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BZRModManager.Converters
{
    internal class TaskNodeStateToBoolConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            try
            {
                if (value is TaskNodeState s && parameter is string p && p != null)
                {
                    if (p.StartsWith("!"))
                        return Enum.Parse<TaskNodeState>(p.Substring(1)) != s;
                    return Enum.Parse<TaskNodeState>(p) == s;
                }
            }
            catch { }
            return false;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
