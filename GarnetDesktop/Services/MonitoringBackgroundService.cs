using CommunityToolkit.Mvvm.Messaging;
using GarnetDesktop.Messages;
using GarnetDesktop.Core.Models;
using GarnetDesktop.Core.Services;

namespace GarnetDesktop.Services;

public sealed class MonitoringBackgroundService : IDisposable
{
    private readonly CancellationTokenSource _cts = new();

    public void Start()
    {
        Task.Run(WorkerLoop);
    }

    private async Task WorkerLoop()
    {
        while (!_cts.IsCancellationRequested)
        {
            try
            {
                var instances = ConfigStore.Load();

                foreach (var instance in instances)
                {
                    instance.Status = WindowsServiceManager.GetStatus(instance);

                    // service not active
                    if (instance.Status != InstanceStatus.Running)
                    {
                        instance.Metrics.CpuPercent = 0;
                        instance.Metrics.MemoryMB = 0;
                        instance.Metrics.Connections = 0;
                    }
                    else
                    {
                        instance.Metrics = MonitoringService.GetMetrics(instance);
                    }
                }

                WeakReferenceMessenger.Default.Send(new MonitoringUpdateMessage(instances));
            }
            catch
            {
            }

            await Task.Delay(2000);
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }
}