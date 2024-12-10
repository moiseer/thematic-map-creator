using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace ThematicMapCreator.Client.Notifications;

public sealed class NotificationBackgroundService : BackgroundService
{
    private readonly SignalRNotificationClient _client;

    public NotificationBackgroundService(SignalRNotificationClient client) => _client = client;

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (true)
        {
            try
            {
                await _client.StartAsync(stoppingToken);
                break;
            }
            catch
            {
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
