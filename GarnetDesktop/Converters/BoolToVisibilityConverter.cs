using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GarnetDesktop.Converters;

[ValueConversion(typeof(bool), typeof(Visibility))]
public sealed class BoolToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// True -> Visible
    /// False  -> Collapsed
    /// </summary>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not bool boolValue)
            return Visibility.Collapsed;

        return boolValue
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    /// <summary>
    /// Visible    -> True
    /// Collapsed  -> False
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not Visibility visibility)
            return Binding.DoNothing;

        return visibility != Visibility.Collapsed;
    }
}
