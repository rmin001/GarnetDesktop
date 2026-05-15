using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GarnetDesktop.Converters;

public sealed class MultiBoolOrToVisibilityConverter : IMultiValueConverter
{
    public object Convert(
        object[] values,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        bool visible =
            values.OfType<bool>().Any(x => x);

        return visible
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    public object[] ConvertBack(
        object value,
        Type[] targetTypes,
        object parameter,
        CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}