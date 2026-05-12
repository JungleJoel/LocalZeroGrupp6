using backend.Data;
using backend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class InitiativeCleanupService : BackgroundService
{
    private readonly ApplicationDbContext _database;
    private readonly IInitiativeService _initiativeService;
    private readonly ILogger<InitiativeCleanupService> _logger;

    public InitiativeCleanupService(ApplicationDbContext database, IInitiativeService initiativeService, ILogger<InitiativeCleanupService> logger)
    {
        _database = database;
        _initiativeService = initiativeService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new(TimeSpan.FromMinutes(1));

        _logger.LogInformation("Initiative Cleanup Service has started.");

        try
        {
            
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await RunCleanupAsync();
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Initiative Cleanup Service is terminating...");
        }
    }

    private async Task RunCleanupAsync()
    {
        var now = DateTime.UtcNow;

        var expiredInitiativeIds = await _database.Initiatives
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
                   
                    await _initiativeService.FinalizeInitiativeAsync(id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error when initiative {Id} where closing.", id);
                }
            }
        }
    }
}