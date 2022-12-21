using Whetstone.CameraMonitor;
using Whetstone.CameraMonitor.AzureImageProcessor;
using Whetstone.CameraMonitor.Service;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddSingleton<IImageProcessor, VisionProcessor>();
    })
    .ConfigureAppConfiguration(configBuilder =>
    {
        var environmentName = Environment.GetEnvironmentVariable("Hosting:Environment");

        configBuilder
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environmentName}.json", true)
            .AddEnvironmentVariables();
    })
    .Build();

host.Run();
