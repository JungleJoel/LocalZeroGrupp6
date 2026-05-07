using backend.Data;
using backend.Models.DTOs;
using backend.Models.DTOs.Requests;
using backend.Models.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;
using backend.Exceptions;
using backend.Interfaces;

namespace backend.Services;

public class InitiativeService : IInitiativeService
{
    private readonly ApplicationDbContext _database;
    private readonly ApplicationDbContext _context;
private readonly IEcoPointService _ecoPointService;
private readonly ILogger<InitiativeService> _logger;

    public InitiativeService(ApplicationDbContext context, IEcoPointService ecoPointService, ILogger<InitiativeService> logger)
{
    _context = context;
    _ecoPointService = ecoPointService;
    _logger = logger;
}

    public async Task<InitiativeDTO> CreateInitiativeAsync(Guid userId, CreateInitiativeRequestDTO request)
    {
        var initiative = new Initiative
        {
            Id = Guid.NewGuid(),
            CommunityId = request.CommunityId,
            CreatedBy = userId,
            Name = request.Name,
            Description = request.Description,
            CategoryId = request.CategoryId,
            PresetId = request.PresetId,
            IsPublic = request.IsPublic,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            StartsAt = request.StartsAt,
            EstimatedEndsAt = request.EstimatedEndsAt,
            EndedAt = null,
            CreatedAt = DateTime.UtcNow,
            EcoPointsPerParticipant = request.EcoPointsPerParticipant
        };

        await _database.Initiatives.AddAsync(initiative);
        await _database.SaveChangesAsync();

        return initiative.Adapt<InitiativeDTO>();
    }

    public async Task<List<InitiativeDTO>> GetInitiativesAsync()
    {
        return (await _context.Initiatives.ToListAsync())
        .Adapt<List<InitiativeDTO>>();
    }

    public async Task<InitiativeDTO> GetInitiative(Guid id)
    {
        var initiative = await _database.Initiatives.FindAsync(id);

        if (initiative == null)
        {
            throw new NotFoundException($"Initiative with id {id} not found");
        }
        return initiative.Adapt<InitiativeDTO>();
    }

    public async Task EndInitiativeAsync(Guid id)
    {
        var initiative = await _database.Initiatives
            .FirstOrDefaultAsync(i => i.Id == id);

        if (initiative == null)
            throw new NotFoundException("Initiative not found");

        if (initiative.EndedAt != null)
            throw new ConflictException("Already ended");

        initiative.EndedAt = DateTime.UtcNow;

        await _database.SaveChangesAsync();
    }

    public async Task CancelInitiativeAsync(Guid id, Guid userId)
    {
        var initiative = await _database.Initiatives
            .FirstOrDefaultAsync(i => i.Id == id);

        if (initiative == null)
            throw new NotFoundException("Initiative not found");

        if (initiative.CreatedBy != userId)
            throw new ConflictException("Not allowed");

        _database.Initiatives.Remove(initiative);

        await _database.SaveChangesAsync();
    }

    public async Task EndInitiativeAsync(Guid id, Guid userId)
    {
        var initiative = await _database.Initiatives
            .FirstOrDefaultAsync(i => i.Id == id);

        if (initiative == null)
            throw new NotFoundException("Initiative not found");

        if (initiative.CreatedBy != userId)
            throw new ConflictException("Not allowed");

        if (initiative.EndedAt != null)
            throw new ConflictException("Already ended");

        initiative.EndedAt = DateTime.UtcNow;

        await _database.SaveChangesAsync();
     }
     public async Task FinalizeInitiativeAsync(Guid initiativeId) //H�r ska po�ng ges till deltagare
     {
        var initiative = await _context.Initiatives
            .Include(i => i.InitiativeParticipators) 
            .FirstOrDefaultAsync(i => i.Id == initiativeId);

        if (initiative == null || initiative.EndedAt != null)
            return;

        initiative.EndedAt = DateTime.UtcNow;

        foreach (var participant in initiative.InitiativeParticipators)
        {
            try
            {
                var pointRequest = new EcoPointRequestDTO(
                    initiative.CommunityId, 
                    participant.UserId, 
                    null, 
                    initiative.EcoPointsPerParticipant ?? 0
                );

                await _ecoPointService.AwardEcoPointsUserAsync(pointRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Not able to give members points.");
            }
        }

        await _context.SaveChangesAsync();
       }
   }