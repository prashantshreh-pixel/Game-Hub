using GameHub.Api.Hubs;
using GameHub.Application.Common.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace GameHub.Api.Services;

public class SignalRNotificationService : INotificationService
{
    private readonly IHubContext<StationHub> _hubContext;

    public SignalRNotificationService(IHubContext<StationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyStationUpdatedAsync(Guid stationId, bool isActive)
    {
        await _hubContext.Clients.All.SendAsync("StationUpdated", stationId, isActive);
    }
}
