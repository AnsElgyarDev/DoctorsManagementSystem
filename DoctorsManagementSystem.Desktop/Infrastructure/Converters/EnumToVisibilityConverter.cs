using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DoctorsManagementSystem.Desktop.Infrastructure.Converters;

/// <summary>
/// Converts an enum value to Visibility.Visible when it matches the string
/// passed via ConverterParameter, otherwise Visibility.Collapsed. Lets XAML
/// drive a "show exactly one of these panels" layout directly off a state enum.
/// </summary>
public class EnumToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null || parameter is null)
            return Visibility.Collapsed;

        return string.Equals(value.ToString(), parameter.ToString(), StringComparison.OrdinalIgnoreCase)
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}