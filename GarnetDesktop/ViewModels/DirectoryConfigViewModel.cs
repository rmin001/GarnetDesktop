using CommunityToolkit.Mvvm.ComponentModel;
using GarnetDesktop.Core.Models;
using System.IO;

namespace GarnetDesktop.ViewModels;

public partial class DirectoryConfigViewModel : ObservableObject
{
    [ObservableProperty]
    private bool enabled;

    [ObservableProperty]
    private string path = string.Empty;

    public void Apply(GarnetDirectoryConfig model)
    {
        model.Enabled = Enabled;
        model.Path = Path;

        if (Enabled && !string.IsNullOrWhiteSpace(Path))
        {
            Directory.CreateDirectory(Path);
        }
    }

    public void Load(GarnetDirectoryConfig model)
    {
        Enabled = model.Enabled;
        Path = model.Path;
    }
}