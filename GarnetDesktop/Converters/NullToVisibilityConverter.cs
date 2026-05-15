using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GarnetDesktop.Converters;

[ValueConversion(typeof(bool), typeof(Visibility))]
public sealed class NullToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// null  -> Collapsed
    /// not-null -> Visible
    /// </summary>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null)
            return Visibility.Collapsed;

        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
