using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DoctorsManagementSystem.Desktop.Infrastructure.Converters;

public class InverseBooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        try
        {
            return (value is true) ? Visibility.Collapsed : Visibility.Visible;
        }
        catch
        {
            return Visibility.Visible;
        }
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => Binding.DoNothing;
}

public class InverseBoolConverter : InverseBooleanToVisibilityConverter
{
}