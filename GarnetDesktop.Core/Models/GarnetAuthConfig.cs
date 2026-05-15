using CommunityToolkit.Mvvm.ComponentModel;

namespace GarnetDesktop.Core.Models;

public partial class GarnetAuthConfig : ObservableObject
{
    [ObservableProperty]
    private AuthMode mode = AuthMode.None;

    [ObservableProperty]
    private string? password;

}
