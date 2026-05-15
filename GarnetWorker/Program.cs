using GarnetWorker;

var host = Host.CreateDefaultBuilder(args)
    .UseContentRoot(AppContext.BaseDirectory)
    .UseWindowsService(options =>
    {
        options.ServiceName = "Garnet Cache Server";
    })
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<GarnetWorkerService>();
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.SetMinimumLevel(LogLevel.Information);
        logging.AddEventLog();
    })
    .Build();

host.Run();