using GarnetDesktop.Core.Helpers;
using GarnetDesktop.Core.Models;
using System.Diagnostics;
using System.ServiceProcess;

namespace GarnetDesktop.Core.Services;

public static partial class WindowsServiceManager
{
    private static readonly string ExecutablePath = Path.Combine(AppContext.BaseDirectory, "Worker", "GarnetWorker.exe");

    public static ServiceResult Install(GarnetInstance instance)
    {
        var args = string.Join(" ", GarnetArgumentBuilder.Build(instance));
        string startMode = instance.AutoStart ? "auto" : "demand";

        string binPath = $"\\\"{ExecutablePath}\\\" {args}";

        string command =
            $"create \"{instance.Name}\" " +
            $"binPath= \"{binPath}\" " +
            $"DisplayName= \"{instance.DisplayName}\" " +
            $"start= {startMode}";

        var result = RunScCommand(command);

        if (!result.Success)
            return result;

        if (!string.IsNullOrWhiteSpace(instance.Description))
        {
            RunScCommand($"description \"{instance.Name}\" \"{instance.Description}\"");
        }

        return result;
    }

    public static ServiceResult Start(GarnetInstance instance)
    {
        var result = RunScCommand($"start \"{instance.Name}\"");
        if (!result.Success)
            return result;

        bool ok = WaitForStatus(instance.Name, ServiceControllerStatus.Running);

        return ok
            ? ServiceResult.Ok("Service started")
            : ServiceResult.Fail("Service did not reach Running state in time");
    }

    public static ServiceResult Stop(GarnetInstance instance)
    {
        var result = RunScCommand($"stop \"{instance.Name}\"");
        if (!result.Success)
            return result;

        bool ok = WaitForStatus(instance.Name, ServiceControllerStatus.Stopped);

        return ok
            ? ServiceResult.Ok("Service stopped")
            : ServiceResult.Fail("Service did not reach Stopped state in time");
    }

    public static ServiceResult Delete(GarnetInstance instance)
    {
        return RunScCommand($"delete \"{instance.Name}\"");
    }

    public static InstanceStatus GetStatus(GarnetInstance instance)
    {
        try
        {
            using var sc = new ServiceController(instance.Name);
            sc.Refresh();

            return sc.Status switch
            {
                ServiceControllerStatus.Running => InstanceStatus.Running,
                ServiceControllerStatus.Stopped => InstanceStatus.Stopped,
                ServiceControllerStatus.StartPending => InstanceStatus.StartPending,
                ServiceControllerStatus.StopPending => InstanceStatus.StopPending,
                ServiceControllerStatus.Paused => InstanceStatus.Paused,
                ServiceControllerStatus.PausePending => InstanceStatus.PausePending,
                ServiceControllerStatus.ContinuePending => InstanceStatus.ContinuePending,
                _ => InstanceStatus.Unknown
            };
        }
        catch
        {
            return InstanceStatus.NotInstalled;
        }
    }

    // -------------------------
    // Core SC executor
    // -------------------------
    private static ServiceResult RunScCommand(string arguments)
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "sc.exe",
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);

            if (process is null)
                return ServiceResult.Fail("Failed to start sc.exe process");

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                return ServiceResult.Fail(
                    $"SC failed with exit code {process.ExitCode}",
                    error + Environment.NewLine + output
                );
            }

            return ServiceResult.Ok(output);
        }
        catch (Exception ex)
        {
            return ServiceResult.Fail(ex.Message);
        }
    }

    // -------------------------
    // Status polling
    // -------------------------
    private static bool WaitForStatus(string serviceName,
        ServiceControllerStatus target,
        int timeoutMs = 10000)
    {
        var sw = Stopwatch.StartNew();

        while (sw.ElapsedMilliseconds < timeoutMs)
        {
            try
            {
                using var sc = new ServiceController(serviceName);
                sc.Refresh();

                if (sc.Status == target)
                    return true;
            }
            catch
            {
                // service may temporarily disappear or fail during transition
            }

            Thread.Sleep(300);
        }

        return false;
    }
    private static string Quote(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "\"\"";

        return value.Contains(' ')
            ? $"\"{value}\""
            : value;
    }

}