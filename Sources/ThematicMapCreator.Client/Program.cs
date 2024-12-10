using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using ThematicMapCreator.Client.Notifications;

namespace ThematicMapCreator.Client;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

        builder.Services.AddSingleton<SignalRNotificationClient>();
        builder.Services.AddHostedService<NotificationBackgroundService>();

        var app = builder.Build();

        await app.RunAsync();
    }
}
