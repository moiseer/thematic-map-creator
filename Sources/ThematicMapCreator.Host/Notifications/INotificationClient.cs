using MediatR;

namespace ThematicMapCreator.Host.Notifications;

public interface INotificationClient
{
    Task NotifyAsync(string notificationType, INotification notification, CancellationToken ct = default);
}
