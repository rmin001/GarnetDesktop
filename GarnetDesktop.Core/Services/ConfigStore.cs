using GarnetDesktop.Core.Models;
using System.Text.Json;

namespace GarnetDesktop.Core.Services;

public static class ConfigStore
{
    private static readonly string Folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GarnetDesktop");

    private static readonly string FilePath = Path.Combine(Folder, "instances.json");

    public static List<GarnetInstance> Load()
    {
        try
        {
            if (!File.Exists(FilePath)) return [];

            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<List<GarnetInstance>>(json) ?? [];
        }
        catch
        {
            return [];
        }
    }

    public static JsonSerializerOptions GetOptions()
    {
        return new JsonSerializerOptions
        {
            WriteIndented = true
        };
    }

    public static void Save(IEnumerable<GarnetInstance> instances, JsonSerializerOptions options)
    {
        Directory.CreateDirectory(Folder);
        string json = JsonSerializer.Serialize(instances, options);

        File.WriteAllText(FilePath, json);
    }
}
