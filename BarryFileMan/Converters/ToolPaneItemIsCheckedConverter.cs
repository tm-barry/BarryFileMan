using Avalonia.Data;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace BarryFileMan.Converters
{
    public class ToolPaneItemIsCheckedConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string valueString && parameter is string paramString
            && targetType.IsAssignableTo(typeof(bool?)))
            {
                return valueString == paramString;
            }
            // converter used for the wrong type
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
