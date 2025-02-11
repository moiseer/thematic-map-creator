using MediatR;

namespace ThematicMapCreator.Host.Notifications;

public interface INotificationClient
{
    Task Notify(string notificationType, INotification notification, CancellationToken ct = default);
}
