using Avalonia.Data.Converters;
using Avalonia.Media;
using BarryFileMan.Helpers;
using System;
using System.Globalization;

namespace BarryFileMan.Converters
{
    public class IntToColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (targetType != typeof(IBrush))
                throw new ArgumentException("Convert - TargetType must be of type Brush");

            if (value != null && value.GetType() != typeof(int))
                throw new ArgumentException("Convert - Value must be of type int");

            return value != null ? new SolidColorBrush(ColorHelper.KellysMaxContrastSet[(int)value % ColorHelper.KellysMaxContrastSet.Count]) : null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
