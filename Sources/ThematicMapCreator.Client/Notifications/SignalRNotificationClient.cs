using System.Text.Json;
using Microsoft.AspNetCore.SignalR.Client;
using Serilog;

namespace ThematicMapCreator.Client.Notifications;

public sealed class SignalRNotificationClient : IAsyncDisposable
{
    private readonly HubConnection _connection;

    public SignalRNotificationClient(IConfiguration configuration, ILogger<SignalRNotificationClient> logger)
    {
        var hubUrl = configuration.GetValue<string>("NotificationHubUrl") ?? throw new ArgumentException("NotificationHubUrl is null", nameof(configuration));

        _connection = new HubConnectionBuilder()
            .ConfigureLogging(logging => logging.AddSerilog())
            .WithAutomaticReconnect()
            .WithStatefulReconnect()
            .WithUrl(hubUrl)
            .Build();

        _connection.On<string, JsonElement>("Notify", (notificationType, notification) =>
        {
            logger.LogDebug("Notification received: {NotificationType}, {Notification}", notificationType, notification);
            return Task.CompletedTask;
        });
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync() => await _connection.DisposeAsync();

    public async Task StartAsync(CancellationToken ct = default) => await _connection.StartAsync(ct);
}
