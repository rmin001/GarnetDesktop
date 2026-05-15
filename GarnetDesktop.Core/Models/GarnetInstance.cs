using CommunityToolkit.Mvvm.ComponentModel;

namespace GarnetDesktop.Core.Models;

public partial class GarnetInstance : ObservableObject
{
    [ObservableProperty]
    private Guid id = Guid.Empty;

    [ObservableProperty]
    private int? processId;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;


    [ObservableProperty]
    private string[]? extraArgs;

    [ObservableProperty]
    private InstanceStatus status = InstanceStatus.NotInstalled;

    [ObservableProperty]
    private bool autoStart = true;


    [ObservableProperty]
    private InstanceMetrics metrics = new();

    [ObservableProperty]
    private int memoryLimitMb = 512;

    [ObservableProperty]
    private GarnetNetworkConfig network = new();

    [ObservableProperty]
    private GarnetAuthConfig auth = new();

    public GarnetDirectoryConfig CheckpointDirectory { get; set; } = new();

    public GarnetDirectoryConfig LogDirectory { get; set; } = new();

    public string DisplayName => $"{Name} ({Network.Port})";

    public bool IsSaved => Id != Guid.Empty;
    public bool IsRunning => Status == InstanceStatus.Running;
    public bool IsStoped => Status == InstanceStatus.Stopped;
    public bool IsNotInstalled => Status == InstanceStatus.NotInstalled || Status == InstanceStatus.Unknown;
    public bool IsInstalled => !IsNotInstalled;
    public bool CanEdit => IsNotInstalled;
    public bool CanDelete => IsNotInstalled || Status == InstanceStatus.Stopped;


    partial void OnStatusChanged(InstanceStatus value)
    {
        OnPropertyChanged(nameof(IsSaved));
        OnPropertyChanged(nameof(CanEdit));
        OnPropertyChanged(nameof(CanDelete));
        OnPropertyChanged(nameof(IsRunning));
        OnPropertyChanged(nameof(IsStoped));
        OnPropertyChanged(nameof(IsInstalled));
    }
}