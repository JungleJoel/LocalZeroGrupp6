using backend.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class InitiativeCleanupService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<InitiativeCleanupService> _logger;

    public InitiativeCleanupService(IServiceProvider services, ILogger<InitiativeCleanupService> logger)
    {
        _services = services;
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
            _logger.LogInformation("Initiative Cleanup Service stoppas...");
        }
    }

    private async Task RunCleanupAsync()
    {
        
        using var scope = _services.CreateScope();
        
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var initiativeService = scope.ServiceProvider.GetRequiredService<InitiativeService>();

        var now = DateTime.UtcNow;

        var expiredInitiativeIds = await context.Initiatives
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