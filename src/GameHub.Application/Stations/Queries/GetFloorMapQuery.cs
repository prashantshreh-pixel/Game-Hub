using Dapper;
using GameHub.Application.Common.Interfaces;
using GameHub.Domain;
using MediatR;

namespace GameHub.Application.Stations.Queries;

public record GetFloorMapQuery : IRequest<IEnumerable<Station>>;

// Optimized with Primary Constructor
public class GetFloorMapQueryHandler(IDbConnectionFactory connectionFactory, ICacheService cache) 
    : IRequestHandler<GetFloorMapQuery, IEnumerable<Station>>
{
    public async Task<IEnumerable<Station>> Handle(GetFloorMapQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = "FloorMap_Stations";
        var cachedStations = await cache.GetAsync<IEnumerable<Station>>(cacheKey);
        if (cachedStations != null) return cachedStations;

        using var connection = connectionFactory.CreateConnection();
        var stations = await connection.QueryAsync<Station>("SELECT Id, Name, IsActive FROM Stations");
        
        await cache.SetAsync(cacheKey, stations, TimeSpan.FromMinutes(5));
        return stations;
    }
}
