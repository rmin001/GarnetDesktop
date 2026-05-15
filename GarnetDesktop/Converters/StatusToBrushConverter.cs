using GarnetDesktop.Core.Models;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace GarnetDesktop.Converters;

public sealed class StatusToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not InstanceStatus status)
            return Brushes.Gray;

        return status switch
        {
            InstanceStatus.Running => Brushes.LimeGreen,
            InstanceStatus.Stopped => Brushes.DarkGray,
            InstanceStatus.NotInstalled => Brushes.Red,
            InstanceStatus.StartPending => Brushes.Orange,
            InstanceStatus.StopPending => Brushes.Orange,
            InstanceStatus.Paused => Brushes.Goldenrod,
            _ => Brushes.Gray
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}