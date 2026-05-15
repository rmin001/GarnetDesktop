using CommunityToolkit.Mvvm.ComponentModel;
using GarnetDesktop.Core.Models;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace GarnetDesktop.ViewModels;

public partial class AuthConfigViewModel : ObservableValidator
{
    public ObservableCollection<AuthMode> Modes { get; } =
      [
        AuthMode.None ,
          AuthMode.Password ,
    ];

    [ObservableProperty]
    private AuthMode mode;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Password is required.")]
    [MinLength(3, ErrorMessage = "Minimum 3 characters.")]
    private string? password;

    public bool IsPassword => Mode == AuthMode.Password;

    public AuthConfigViewModel()
    {
        Mode = AuthMode.None;
    }

    partial void OnModeChanged(AuthMode value)
    {
        OnPropertyChanged(nameof(IsPassword));
    }
}
