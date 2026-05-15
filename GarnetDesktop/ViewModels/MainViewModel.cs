using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GarnetDesktop.Core.Models;
using GarnetDesktop.Core.Services;
using GarnetDesktop.Enums;
using GarnetDesktop.Messages;
using GarnetDesktop.Views.Dialogs;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace GarnetDesktop.ViewModels;

public sealed partial class MainViewModel
    : ObservableObject,
      IRecipient<MonitoringUpdateMessage>
{
    public ObservableCollection<GarnetInstance> Instances { get; } = [];

    [ObservableProperty]
    private GarnetInstance? selectedInstance;

    public ObservableCollection<double> CpuValues { get; } = [];
    public ObservableCollection<double> MemoryValues { get; } = [];

    public ISeries[] CpuSeries { get; }
    public ISeries[] MemorySeries { get; }

    public bool CanToggleService =>
    SelectedInstance is not null &&
    (
        SelectedInstance.Status == InstanceStatus.Running ||
        SelectedInstance.Status == InstanceStatus.Stopped ||
        SelectedInstance.Status == InstanceStatus.NotInstalled

    );

    public bool CanDelete => SelectedInstance is not null && SelectedInstance.CanDelete;

    public string ToggleServiceText => SelectedInstance?.Status == InstanceStatus.Running
            ? "Stop"
            : SelectedInstance?.Status == InstanceStatus.Stopped ? "Start"
            : "Install";

    public Brush SelectedStatusBrush =>
        SelectedInstance?.Status switch
        {
            InstanceStatus.Running => Brushes.LimeGreen,
            InstanceStatus.Stopped => Brushes.OrangeRed,
            InstanceStatus.StartPending => Brushes.Gold,
            InstanceStatus.StopPending => Brushes.Gold,
            InstanceStatus.Paused => Brushes.DarkOrange,
            InstanceStatus.NotInstalled => Brushes.Gray,
            _ => Brushes.DimGray
        };

    public MainViewModel()
    {
        CpuSeries =
        [
            new LineSeries<double>
            {
                Values = CpuValues
            }
        ];

        MemorySeries =
        [
            new LineSeries<double>
            {
                Values = MemoryValues
            }
        ];

        WeakReferenceMessenger.Default.Register(this);

        LoadInstances();
    }

    partial void OnSelectedInstanceChanged(GarnetInstance? value)
    {
        OnPropertyChanged(nameof(SelectedStatusBrush));
        OnPropertyChanged(nameof(CanToggleService));
        OnPropertyChanged(nameof(ToggleServiceText));
        OnPropertyChanged(nameof(CanDelete));

        CpuValues.Clear();
        MemoryValues.Clear();

        UpdateCharts();
    }

    public async void Receive(MonitoringUpdateMessage message)
    {
        await App.Current.Dispatcher.InvokeAsync(() =>
        {
            foreach (var updated in message.Value)
            {
                var existing =
                    Instances.FirstOrDefault(x => x.Id == updated.Id);

                if (existing is null)
                    continue;

                existing.Status = updated.Status;
                existing.ProcessId = updated.ProcessId;

                existing.Metrics.CpuPercent = updated.Metrics.CpuPercent;
                existing.Metrics.MemoryMB = updated.Metrics.MemoryMB;
                existing.Metrics.Connections = updated.Metrics.Connections;
            }

            OnPropertyChanged(nameof(SelectedStatusBrush));
            OnPropertyChanged(nameof(CanToggleService));
            OnPropertyChanged(nameof(ToggleServiceText));
            OnPropertyChanged(nameof(CanDelete));

            UpdateCharts();
        });
    }
    [RelayCommand]
    private async Task ToggleService()
    {
        if (SelectedInstance is null)
            return;

        ServiceResult result;

        if (SelectedInstance.Status == InstanceStatus.Running)
        {
            result = WindowsServiceManager.Stop(SelectedInstance);
        }
        else if (SelectedInstance.Status == InstanceStatus.Stopped)
        {
            result = WindowsServiceManager.Start(SelectedInstance);
        }
        else
        {
            result = WindowsServiceManager.Install(SelectedInstance);
        }

        if (!result.Success)
        {
            MessageBox.Show(
            result.Output,
            "Error",
            MessageBoxButton.OK,
            MessageBoxImage.Error);

            Debug.WriteLine(result.Error);
            return;
        }

        await Task.Delay(500);
    }

    [RelayCommand]
    private void Reload()
    {
        LoadInstances();
    }

    [RelayCommand]
    private void Add()
    {
        var instance = new GarnetInstance();

        var vm = new InstanceViewModel(instance);

        var window = new InstanceDialog
        {
            DataContext = vm
        };

        var result = window.ShowDialog();

        if (result != true)
            return;

        if (vm.Result == InstanceDialogResult.Saved)
        {
            Instances.Add(instance);

            Save();
        }
    }

    [RelayCommand]
    private void ShowDetails(GarnetInstance instance)
    {
        if (instance is null)
            return;

        var vm = new InstanceViewModel(instance);

        var dialog = new InstanceDetailsDialog
        {
            DataContext = vm
        };

        dialog.ShowDialog();
    }

    [RelayCommand]
    private async Task Start()
    {
        if (SelectedInstance is null)
            return;

        var result =
            WindowsServiceManager.Start(SelectedInstance);

        if (!result.Success)
        {
            Debug.WriteLine(result.Error);

            MessageBox.Show(
              result.Error ?? "Start failed.",
              "Error",
              MessageBoxButton.OK,
              MessageBoxImage.Error);

            return;
        }

        await Task.Delay(500);
    }

    [RelayCommand]
    private async Task Stop()
    {
        if (SelectedInstance is null)
            return;

        var result =
            WindowsServiceManager.Stop(SelectedInstance);

        if (!result.Success)
        {
            Debug.WriteLine(result.Error);

            MessageBox.Show(
            result.Error ?? "Stop failed.",
            "Error",
            MessageBoxButton.OK,
            MessageBoxImage.Error);

            return;
        }

        await Task.Delay(500);
    }

    [RelayCommand]
    private void Edit(GarnetInstance instance)
    {
        if (instance is null)
            return;

        var vm = new InstanceViewModel(instance);

        var dialog = new InstanceDialog
        {
            DataContext = vm
        };

        var result = dialog.ShowDialog();

        if (result != true)
            return;

        if (vm.Result == InstanceDialogResult.Saved)
        {
            Save();

            CollectionViewSource
                .GetDefaultView(Instances)
                .Refresh();
        }
        else if (vm.Result == InstanceDialogResult.Deleted)
        {
            DeleteInstance(instance);

        }
    }

    [RelayCommand]
    private void Delete()
    {
        if (SelectedInstance is null)
            return;

        var confirm =
            MessageBox.Show(
                $"Delete instance '{SelectedInstance.Name}' ?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

        if (confirm != MessageBoxResult.Yes)
            return;

        DeleteInstance(SelectedInstance);
    }


    private void LoadInstances()
    {
        Instances.Clear();

        foreach (var item in ConfigStore.Load())
        {
            Instances.Add(item);
        }
    }

    private void UpdateCharts()
    {
        if (SelectedInstance is null)
            return;

        CpuValues.Add(
            SelectedInstance.Metrics.CpuPercent);

        MemoryValues.Add(
            SelectedInstance.Metrics.MemoryMB);

        while (CpuValues.Count > 30)
        {
            CpuValues.RemoveAt(0);
        }

        while (MemoryValues.Count > 30)
        {
            MemoryValues.RemoveAt(0);
        }
    }
    private bool DeleteInstance(GarnetInstance instance)
    {
        if (instance.IsInstalled)
        {
            var result = WindowsServiceManager.Delete(instance);

            if (!result.Success)
            {
                Debug.WriteLine(result.Error);

                MessageBox.Show(
                    result.Error ?? "Delete failed.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return false;
            }
        }

        Instances.Remove(instance);

        if (ReferenceEquals(SelectedInstance, instance))
        {
            SelectedInstance = null;
        }

        Save();
        return true;
    }


    private void Save()
    {
        ConfigStore.Save(
            Instances,
            ConfigStore.GetOptions());
    }
}