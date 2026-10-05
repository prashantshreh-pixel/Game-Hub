using Dapper;
using GameHub.Application.Common.Interfaces;
using MediatR;

namespace GameHub.Application.Sessions.Commands;

public record StartSessionCommand(Guid StationId, Guid? CustomerId = null) : IRequest<Guid>;

// Optimized with Primary Constructor
public class StartSessionCommandHandler(
    IDbConnectionFactory connectionFactory, 
    ICacheService cache, 
    INotificationService notificationService) 
    : IRequestHandler<StartSessionCommand, Guid>
{
    public async Task<Guid> Handle(StartSessionCommand request, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try 
        {
            var sessionId = Guid.NewGuid();
            var insertSql = "INSERT INTO Sessions (Id, StationId, StartTime, CustomerId) VALUES (@Id, @StationId, @StartTime, @CustomerId)";
            await connection.ExecuteAsync(insertSql, new { Id = sessionId, request.StationId, StartTime = DateTime.UtcNow, request.CustomerId }, transaction);

            await connection.ExecuteAsync("UPDATE Stations SET IsActive = 1 WHERE Id = @StationId", new { request.StationId }, transaction);

            transaction.Commit();

            await cache.RemoveAsync("FloorMap_Stations");
            await notificationService.NotifyStationUpdatedAsync(request.StationId, true);

            return sessionId;
        }
        catch { transaction.Rollback(); throw; }
    }
}
