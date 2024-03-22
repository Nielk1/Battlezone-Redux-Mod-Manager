//https://gist.github.com/awatertrevi/68924981bdea1800f5af162e4eb2b1f5

using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BZRModManager.Converters
{
    public class ValueConverterGroup : List<IValueConverter>, IValueConverter
    {
        private string[] _parameters;
        private bool _shouldReverse = false;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ExtractParameters(parameter);

            if (_shouldReverse)
            {
                Reverse();
                _shouldReverse = false;
            }

            return this.Aggregate(value, (current, converter) => converter.Convert(current, targetType, GetParameter(converter), culture));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ExtractParameters(parameter);

            Reverse();
            _shouldReverse = true;

            return this.Aggregate(value, (current, converter) => converter.ConvertBack(current, targetType, GetParameter(converter), culture));
        }

        private void ExtractParameters(object parameter)
        {
            if (parameter != null)
                _parameters = Regex.Split(parameter.ToString(), @"(?<!\\),");
        }

        private string GetParameter(IValueConverter converter)
        {
            if (_parameters == null)
                return null;

            var index = IndexOf(converter as IValueConverter);
            string parameter;

            try
            {
                parameter = _parameters[index];
            }

            catch (IndexOutOfRangeException ex)
            {
                parameter = null;
            }

            if (parameter != null)
                parameter = Regex.Unescape(parameter);

            return parameter;
        }
    }
}
