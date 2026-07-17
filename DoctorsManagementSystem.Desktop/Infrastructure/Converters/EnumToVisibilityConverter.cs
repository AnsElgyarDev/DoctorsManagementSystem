using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DoctorsManagementSystem.Desktop.Infrastructure.Converters;

public class EnumToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        try
        {
            if (value is null || parameter is null)
                return Visibility.Collapsed;

            return string.Equals(value.ToString(), parameter.ToString(), StringComparison.OrdinalIgnoreCase)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
        catch
        {
            // Never let a converter exception propagate into the binding engine.
            return Visibility.Collapsed;
        }
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => Binding.DoNothing;
}