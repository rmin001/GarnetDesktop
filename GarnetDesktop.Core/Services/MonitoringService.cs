using GarnetDesktop.Core.Models;
using System.Diagnostics;
using System.Management;
using System.Net.NetworkInformation;

namespace GarnetDesktop.Core.Services;

public sealed class MonitoringService
{
    public static InstanceMetrics GetMetrics(GarnetInstance instance)
    {
        var metrics = new InstanceMetrics();

        try
        {
            int pid = GetServiceProcessId(instance.Name);

            if (pid <= 0)
            {
                instance.ProcessId = null;
                instance.Status = InstanceStatus.Stopped;

                return metrics;
            }

            instance.ProcessId = pid;

            using var process = Process.GetProcessById(pid);
            metrics.MemoryMB = process.WorkingSet64 / (1024f * 1024f);

            metrics.CpuPercent = GetCpuUsage(process);

            metrics.Connections =
                IPGlobalProperties.GetIPGlobalProperties()
                    .GetActiveTcpConnections()
                    .Count(x => x.LocalEndPoint.Port == instance.Network.Port);

            instance.Metrics = metrics;
        }
        catch
        {
            instance.Status = InstanceStatus.Unknown;
            instance.ProcessId = null;
            instance.Metrics = new InstanceMetrics();
        }

        return metrics;
    }

    private static int GetServiceProcessId(string serviceName)
    {
        using var searcher = new ManagementObjectSearcher($"SELECT ProcessId FROM Win32_Service WHERE Name = '{serviceName}'");

        foreach (ManagementObject obj in searcher.Get())
        {
            return Convert.ToInt32(obj["ProcessId"]);
        }

        return -1;
    }

    private static float GetCpuUsage(Process process)
    {
        using var counter =
            new PerformanceCounter(
                "Process",
                "% Processor Time",
                process.ProcessName,
                true);

        counter.NextValue();

        Thread.Sleep(200);

        return counter.NextValue() / Environment.ProcessorCount;
    }
}