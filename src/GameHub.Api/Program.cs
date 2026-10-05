using GameHub.Api.Hubs;
using GameHub.Api.Services;
using GameHub.Application;
using GameHub.Application.Common.Interfaces;
using GameHub.Application.Sessions.Commands;
using GameHub.Application.Stations.Queries;
using GameHub.Infrastructure.Persistence;
using GameHub.Infrastructure.Services;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Memory Cache & Dapper
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICacheService, MemoryCacheService>();
builder.Services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();

// SignalR & Background Seeder
builder.Services.AddTransient<INotificationService, SignalRNotificationService>();
builder.Services.AddHostedService<DbSeederHostedService>();
builder.Services.AddSignalR();

// MediatR & CORS
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
builder.Services.AddCors(options =>
    options.AddPolicy("BlazorCors", policy =>
        policy.SetIsOriginAllowed(_ => true).AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("BlazorCors");

// ==========================================
// OPTIMIZED MINIMAL APIs (No Controllers needed)
// ==========================================
var api = app.MapGroup("/api");

api.MapGet("/stations/floormap", async (IMediator mediator) => 
    Results.Ok(await mediator.Send(new GetFloorMapQuery())));

api.MapPost("/sessions/start", async (StartSessionCommand command, IMediator mediator) => 
    Results.Ok(new { SessionId = await mediator.Send(command) }));

app.MapHub<StationHub>("/stationhub");

app.Run();
