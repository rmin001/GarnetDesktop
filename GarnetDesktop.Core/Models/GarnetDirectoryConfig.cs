using CommunityToolkit.Mvvm.ComponentModel;

namespace GarnetDesktop.Core.Models;

public partial class GarnetDirectoryConfig : ObservableObject
{
    [ObservableProperty]
    private bool enabled;

    [ObservableProperty]
    private string path = string.Empty;
}