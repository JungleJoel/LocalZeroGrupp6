using backend.Data;
using backend.Models.DTOs;
using backend.Models.DTOs.Requests;
using backend.Models.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;
using backend.Exceptions;

namespace backend.Services;

public class InitiativeService
{
    private readonly ApplicationDbContext _database;

    public InitiativeService(ApplicationDbContext database)
    {
        _database = database;
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
            CreatedAt = DateTimeOffset.UtcNow,
            EcoPointsPerParticipant = request.EcoPointsPerParticipant
        };

        await _database.Initiatives.AddAsync(initiative);
        await _database.SaveChangesAsync();

        return initiative.Adapt<InitiativeDTO>();
    }

    public async Task<List<InitiativeDTO>> GetInitiativesAsync()
    {
        return (await _database.Initiatives.ToListAsync())
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

        initiative.EndedAt = DateTimeOffset.UtcNow;

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
}