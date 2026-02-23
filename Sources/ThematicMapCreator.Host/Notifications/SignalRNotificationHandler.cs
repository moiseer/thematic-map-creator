using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace ThematicMapCreator.Host.Notifications;

public sealed class SignalRNotificationHandler<TNotification> : INotificationHandler<TNotification>
    where TNotification : INotification
{
    private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;

    public SignalRNotificationHandler(IHubContext<NotificationHub, INotificationClient> hubContext) => _hubContext = hubContext;

    /// <inheritdoc/>
    public async Task Handle(TNotification notification, CancellationToken cancellationToken) =>
        await _hubContext.Clients.All.NotifyAsync(notification.GetType().Name, notification, cancellationToken);
}
