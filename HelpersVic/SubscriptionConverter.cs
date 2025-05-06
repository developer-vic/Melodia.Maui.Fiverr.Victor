using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MelodiaTherapy.HelpersVic
{
    public class PercentageConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is double width && parameter is string percentageStr)
            {
                if (double.TryParse(percentageStr, out double percentage))
                {
                    return width * percentage;
                }
            }
            return value;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Converter to determine background color based on selection state
    public class SelectedBackgroundConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isSelected)
            {
                return isSelected
                    ? "#76cec5"
                    : "#17142b";
            }
            return "#17142b";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}