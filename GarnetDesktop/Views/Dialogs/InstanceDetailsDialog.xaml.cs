using GarnetDesktop.ViewModels;
using MahApps.Metro.Controls;
using System.Windows;

namespace GarnetDesktop.Views.Dialogs;


public partial class InstanceDetailsDialog : MetroWindow
{
    public InstanceDetailsDialog()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is InstanceViewModel vm)
        {
            //...
        }
    }
}
