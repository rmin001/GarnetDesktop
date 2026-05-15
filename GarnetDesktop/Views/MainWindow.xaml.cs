using GarnetDesktop.ViewModels;
using MahApps.Metro.Controls;
using System.Windows;

namespace GarnetDesktop.Views;

public partial class MainWindow : MetroWindow
{
    private readonly MainViewModel _viewModel;

    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        DataContext = _viewModel;
        Loaded += OnLoaded;
    }
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        //_viewModel.LoadInstances(); --→ load inside the vm.ctor 
    }

    private void InstancesList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (_viewModel.SelectedInstance is null)
            return;

        _viewModel.EditCommand.Execute(null);
    }
}