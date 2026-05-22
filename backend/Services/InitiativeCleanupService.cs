using backend.Data;
using backend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class InitiativeCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<InitiativeCleanupService> _logger;

    public InitiativeCleanupService(IServiceScopeFactory scopeFactory, ILogger<InitiativeCleanupService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new(TimeSpan.FromMinutes(1));
        _logger.LogInformation("Initiative Cleanup Service has started.");
        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
                await RunCleanupAsync();
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Initiative Cleanup Service is terminating...");
        }
    }

    private async Task RunCleanupAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var initiativeService = scope.ServiceProvider.GetRequiredService<IInitiativeService>();

        var now = DateTime.UtcNow;
        var expiredInitiativeIds = await database.Initiatives
            .Where(i => i.EndedAt == null && i.EstimatedEndsAt < now)
            .Select(i => i.Id)
            .ToListAsync();

        if (expiredInitiativeIds.Any())
        {
            _logger.LogInformation("Found {Count} initiatives to end.", expiredInitiativeIds.Count);
            foreach (var id in expiredInitiativeIds)
            {
                try
                {
                    await initiativeService.FinalizeInitiativeAsync(id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error when initiative {Id} where closing.", id);
                }
            }
        }
    }
}