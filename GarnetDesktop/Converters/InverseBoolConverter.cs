using System.Globalization;
using System.Windows.Data;

namespace GarnetDesktop.Converters;

[ValueConversion(typeof(bool), typeof(bool))]
public sealed class InverseBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool boolValue
            ? !boolValue
            : Binding.DoNothing;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool boolValue
            ? !boolValue
            : Binding.DoNothing;
    }
}
