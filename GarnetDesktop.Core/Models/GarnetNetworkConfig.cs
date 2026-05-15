using CommunityToolkit.Mvvm.ComponentModel;

namespace GarnetDesktop.Core.Models;

public partial class GarnetNetworkConfig : ObservableObject
{
    public string BindAddressDisplay => $"{NicName} ({BindAddress})";

    [ObservableProperty]
    private string bindAddress = "127.0.0.1";

    [ObservableProperty]
    private string nicName = "Loopback";

    [ObservableProperty]
    private int port = 3278;


    partial void OnBindAddressChanged(string value)
    {
        OnPropertyChanged(nameof(BindAddressDisplay));
    }

    partial void OnNicNameChanged(string value)
    {
        OnPropertyChanged(nameof(BindAddressDisplay));
    }
}
