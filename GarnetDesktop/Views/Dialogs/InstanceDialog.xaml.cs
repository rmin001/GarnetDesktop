using GarnetDesktop.ViewModels;
using MahApps.Metro.Controls;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace GarnetDesktop.Views.Dialogs;

public partial class InstanceDialog : MetroWindow
{
    private static readonly Regex _numberRegex = NumberRegexPattern();

    public InstanceDialog()
    {
        InitializeComponent();

        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is InstanceViewModel vm)
        {
            PasswordBox.Password = vm.Auth.Password;
        }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;


    private void NumberOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = _numberRegex.IsMatch(e.Text);
    }

    [GeneratedRegex("[^0-9]+")]
    private static partial Regex NumberRegexPattern();

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is InstanceViewModel vm)
        {
            vm.Auth.Password = PasswordBox.Password;
        }
    }

}
