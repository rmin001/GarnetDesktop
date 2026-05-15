using Garnet;

namespace GarnetWorker;

public class GarnetWorkerService(ILogger<GarnetWorkerService> _logger) : BackgroundService
{
    private GarnetServer? _server;

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            _server = new GarnetServer(Environment.GetCommandLineArgs());
            _server.Start();

            return Task.CompletedTask; // IMPORTANT: do not block
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to start Garnet: {ex}");
            throw;
        }
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // keep service alive
        return Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Stopping Garnet server...");

            _server?.Dispose();

            _logger.LogInformation("Garnet server stopped.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping Garnet server");
        }

        return base.StopAsync(cancellationToken);
    }
}