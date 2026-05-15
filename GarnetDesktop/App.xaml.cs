using GarnetDesktop.Services;
using GarnetDesktop.ViewModels;
using GarnetDesktop.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
namespace GarnetDesktop;

public partial class App : Application
{
    private MonitoringBackgroundService? _monitoring;
    private IHost? _host;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _monitoring = new MonitoringBackgroundService();
        _monitoring.Start();

        _host = Host.CreateDefaultBuilder(e.Args)
           .ConfigureAppConfiguration((context, config) =>
           {
               config.SetBasePath(AppContext.BaseDirectory);
               config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
           })
           .ConfigureServices((context, services) =>
           {

               services.AddSingleton<MainViewModel>();
               services.AddSingleton<MainWindow>();
           })
           .Build();

        await _host.StartAsync().ConfigureAwait(true);

        var window = _host.Services.GetRequiredService<MainWindow>();
        MainWindow = window;
        window.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _monitoring?.Dispose();

        base.OnExit(e);
    }
}
