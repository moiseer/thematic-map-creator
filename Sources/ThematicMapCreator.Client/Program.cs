using Serilog;
using ThematicMapCreator.Client.Notifications;

namespace ThematicMapCreator.Client;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args);
        builder.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

        builder.ConfigureServices(services =>
        {
            services.AddSingleton<SignalRNotificationClient>();
            services.AddHostedService<NotificationBackgroundService>();
        });

        await builder.RunConsoleAsync();
    }
}
