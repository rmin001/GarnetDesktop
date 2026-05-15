using CommunityToolkit.Mvvm.ComponentModel;

namespace GarnetDesktop.Core.Models;

public sealed partial class InstanceMetrics : ObservableObject
{
    [ObservableProperty]
    private float cpuPercent;

    [ObservableProperty]
    private float memoryMB;

    [ObservableProperty]
    private int connections;
}
