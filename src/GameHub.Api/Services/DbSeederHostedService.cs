using System.Data;
using Dapper;
using GameHub.Application.Common.Interfaces;
using Microsoft.Extensions.Hosting;

namespace GameHub.Api.Services;

public class DbSeederHostedService : IHostedService
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DbSeederHostedService(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Stations");
        if (count == 0)
        {
            await connection.ExecuteAsync("INSERT INTO Stations (Id, Name, IsActive) VALUES (@Id, @Name, 0)", new[] 
            {
                new { Id = Guid.NewGuid(), Name = "PS5 - Station 1" },
                new { Id = Guid.NewGuid(), Name = "PS5 - Station 2" },
                new { Id = Guid.NewGuid(), Name = "Xbox - Station 3" },
                new { Id = Guid.NewGuid(), Name = "PC - Station 4" }
            });
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
