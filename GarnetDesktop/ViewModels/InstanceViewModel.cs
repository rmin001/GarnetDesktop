using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GarnetDesktop.Core.Models;
using GarnetDesktop.Enums;
using Microsoft.Win32;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;

namespace GarnetDesktop.ViewModels;

public partial class InstanceViewModel : ObservableValidator
{
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Name is required.")]
    [MinLength(3, ErrorMessage = "Minimum 3 characters.")]
    private string name = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private int memoryLimitMb = 512;

    [ObservableProperty]
    private bool autoStart;

    public GarnetInstance Instance { get; }

    public NetworkConfigViewModel Network { get; }
    public AuthConfigViewModel Auth { get; }
    public DirectoryConfigViewModel CheckpointDirectory { get; }
    public DirectoryConfigViewModel LogDirectory { get; }

    public bool HasStorageDirectories =>
            CheckpointDirectory.Enabled ||
            LogDirectory.Enabled;

    public InstanceDialogResult Result { get; private set; }

    public InstanceViewModel(GarnetInstance instance)
    {
        Instance = instance;

        Name = instance.Name.Trim();
        Description = instance.Description;
        MemoryLimitMb = instance.MemoryLimitMb;
        AutoStart = instance.AutoStart;

        // NETWORK VM
        Network = new NetworkConfigViewModel();
        Network.Port = instance.Network.Port;

        var bind =
            Network.BindAddresses
                   .FirstOrDefault(x =>
                        x.Address == instance.Network.BindAddress);

        if (bind != null)
            Network.SelectedBindAddress = bind;

        // AUTH VM
        Auth = new AuthConfigViewModel();
        Auth.Mode = instance.Auth.Mode;
        Auth.Password = instance.Auth.Mode == AuthMode.Password ? instance.Auth.Password : string.Empty;

        // DIRECTORIES
        CheckpointDirectory = new DirectoryConfigViewModel();
        LogDirectory = new DirectoryConfigViewModel();

        CheckpointDirectory.Load(Instance.CheckpointDirectory);
        LogDirectory.Load(Instance.LogDirectory);

        CheckpointDirectory.PropertyChanged += OnDirectoryChanged;
        LogDirectory.PropertyChanged += OnDirectoryChanged;

    }

    [RelayCommand]
    private void Save(Window window)
    {
        ValidateAllProperties();

        if (HasErrors)
            return;

        Apply();

        Result = InstanceDialogResult.Saved;

        window.DialogResult = true;
        window.Close();
    }

    [RelayCommand]
    private void Delete(Window window)
    {
        var confirm =
            MessageBox.Show(
                $"Delete instance '{Instance.Name}' ?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

        if (confirm != MessageBoxResult.Yes)
            return;

        Result = InstanceDialogResult.Deleted;

        window.DialogResult = true;
        window.Close();
    }
    private void Apply()
    {
        if (Instance.Id == Guid.Empty)
        {
            Instance.Id = Guid.CreateVersion7();
        }

        Instance.Name = Name.Trim();
        Instance.Description = Description;
        Instance.AutoStart = AutoStart;

        // enforce minimum safe memory (Tsavorite requirement)
        Instance.MemoryLimitMb = Math.Max(128, MemoryLimitMb);

        // NETWORK APPLY
        Instance.Network.BindAddress = Network.BindAddress;
        Instance.Network.NicName = Network.NicName;
        Instance.Network.Port = Network.Port;

        // AUTH APPLY
        Instance.Auth.Mode = Auth.Mode;
        Instance.Auth.Password = Auth.Mode == AuthMode.Password ? Auth.Password : string.Empty;

        // DIRECTORIES
        CheckpointDirectory.Apply(Instance.CheckpointDirectory);
        LogDirectory.Apply(Instance.LogDirectory);

    }


    [RelayCommand]
    private void BrowseCheckpointDirectory()
    {
        var path = SelectFolder();

        if (path is null)
            return;

        CheckpointDirectory.Path = path;
    }

    [RelayCommand]
    private void BrowseLogDirectory()
    {
        var path = SelectFolder();

        if (path is null)
            return;

        LogDirectory.Path = path;
    }
    private static string? SelectFolder()
    {
        var dialog = new OpenFolderDialog();

        return dialog.ShowDialog() == true
            ? dialog.FolderName
            : null;
    }

    [RelayCommand]
    private void Cancel(Window window)
    {
        Result = InstanceDialogResult.None;
        window.DialogResult = false;
        window.Close();
    }

    [RelayCommand]
    private void Close(Window window)
    {
        window.Close();
    }

    partial void OnNameChanged(string value)
    {
        ValidateProperty(value, nameof(Name));
    }
    private void OnDirectoryChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(GarnetDirectoryConfig.Enabled))
        {
            OnPropertyChanged(nameof(HasStorageDirectories));
        }
    }

}