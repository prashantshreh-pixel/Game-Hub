namespace GameHub.Application.Common.Interfaces;

public interface INotificationService
{
    Task NotifyStationUpdatedAsync(Guid stationId, bool isActive);
}
