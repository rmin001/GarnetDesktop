using GarnetDesktop.Core.Models;

namespace GarnetDesktop.Core.Helpers;

public static class GarnetArgumentBuilder
{
    public static string[] Build(GarnetInstance instance)
    {
        var args = new List<string>(16)
        {
            $"--bind {instance.Network.BindAddress}" ,
            $"--port {instance.Network.Port}",
        };

        // MEMORY LIMIT (important addition)
        if (instance.MemoryLimitMb is > 0)
        {
            var memoryBytes = GarnetMemoryPolicy.Normalize(instance.MemoryLimitMb);

            args.Add($"--memory {memoryBytes}");
        }

        args.AddRange(BuildAuth(instance));

        if (!string.IsNullOrWhiteSpace(instance.Auth.Password))
        {
            args.Add($"--password {Quote(instance.Auth.Password)}");
        }

        if (instance.ExtraArgs is { Length: > 0 })
        {
            args.AddRange(instance.ExtraArgs
                .Where(a => !string.IsNullOrWhiteSpace(a)));
        }

        // IMPORTANT: no quotes inside values

        if (instance.CheckpointDirectory.Enabled)
        {
            args.Add($"--checkpointdir {Quote(instance.CheckpointDirectory.Path)}");
        }

        if (instance.LogDirectory.Enabled)
        {
            args.Add("--storage-tier true");
            args.Add($"--logdir {Quote(instance.LogDirectory.Path)}");
        }

        return args.ToArray();
    }

    private static IEnumerable<string> BuildAuth(GarnetInstance instance)
    {
        return instance.Auth.Mode switch
        {
            AuthMode.None => ["--auth", "NoAuth"],
            AuthMode.Password => ["--auth", "Password"],
            _ => ["--auth", "NoAuth"]
        };
    }

    private static string Quote(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "\"\"";

        // escape quotes for nested SCM parsing
        value = value.Replace("\"", "\\\"");

        return value.Contains(' ') || value.Contains('"')
            ? $"\\\"{value}\\\""
            : value;
    }
}