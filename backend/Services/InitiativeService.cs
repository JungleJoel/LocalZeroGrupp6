using backend.Data;
using backend.Models.DTOs;
using backend.Models.DTOs.Requests;
using backend.Models.Entities;
using backend.Settings;
using Mapster;
using Microsoft.EntityFrameworkCore;
using backend.Exceptions;
using backend.Interfaces;
using backend.Services.EcoPoint;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace backend.Services;

public class InitiativeService : IInitiativeService
{
    private readonly ApplicationDbContext _database;
    private readonly EcoPointCommandInvoker _ecoPointCommandInvoker;
    private readonly ILogger<InitiativeService> _logger;

    public InitiativeService(ApplicationDbContext context, EcoPointCommandInvoker ecoPointCommandInvoker,
        ILogger<InitiativeService> logger)
    {
        _database = context;
        _ecoPointCommandInvoker = ecoPointCommandInvoker;
        _logger = logger;
    }

    public async Task<InitiativeDTO> CreateInitiativeAsync(Guid userId, CreateInitiativeRequestDTO request)
    {
        if (request.EcoPointsPerParticipant <= 0)
            throw new ArgumentException("Eco points per participant must be greater than zero.");

        if (request.EcoPointsPerParticipant > EcoPointsConfig.Instance().MaxEcoPointsPerParticipant)
            throw new ArgumentException($"Eco points per participant cannot exceed {EcoPointsConfig.Instance().MaxEcoPointsPerParticipant}.");

        if (request.EstimatedEndsAt < DateTime.UtcNow)
            throw new ArgumentException("Estimated end date must be in the future.");

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

        return ToInitiativeDto(initiative, userId);
    }

    public async Task<List<InitiativeDTO>> GetInitiativesAsync(Guid userId)
    {
        var initiatives = await _database.Initiatives
            .Include(i => i.InitiativeLikes)
            .Include(i => i.InitiativeParticipators)
            .ToListAsync();

        return initiatives
            .Select(i => ToInitiativeDto(i, userId))
            .ToList();
    }

    public async Task<InitiativeDTO> GetInitiativeAsync(Guid initiativeId, Guid userId)
    {
        var initiative = await _database.Initiatives
            .Include(i => i.InitiativeLikes)
            .Include(i => i.InitiativeParticipators)
            .FirstOrDefaultAsync(i => i.Id == initiativeId);

        if (initiative is null)
            throw new NotFoundException("Initiative not found");

        return ToInitiativeDto(initiative, userId);
    }

    public async Task RemoveInitiativeAsync(Guid inititativeId, Guid userId)
    {
        var initiative = await _database.Initiatives
            .Include(i => i.InitiativeParticipators)
            .FirstOrDefaultAsync(i => i.Id == inititativeId);

        if (initiative == null)
            throw new NotFoundException("Initiative not found");

        if (initiative.CreatedBy != userId)
            throw new ConflictException("Not allowed");

        foreach (var participator in initiative.InitiativeParticipators)
        {
            await LeaveInitiativeAsync(inititativeId, participator.UserId);
        }

        _database.Initiatives.Remove(initiative);

        await _database.SaveChangesAsync();
    }

    public async Task<List<InitiativeDTO>> GetByCommunityIdAsync(Guid communityId, Guid userId)
    {
        var results = await _database.Initiatives
            .Include(i => i.InitiativeLikes)
            .Include(i => i.InitiativeParticipators)
            .Where(i => i.CommunityId == communityId && i.EndedAt == null)
            .OrderBy(i => i.StartsAt)
            .ToListAsync();

        return results
            .Select(i => ToInitiativeDto(i, userId))
            .ToList();
    }

    public async Task EndInitiativeAsync(Guid initiativeId, Guid userId)
    {
        var initiative = await _database.Initiatives
            .FirstOrDefaultAsync(i => i.Id == initiativeId);

        if (initiative == null)
            throw new NotFoundException("Initiative not found");

        if (initiative.CreatedBy != userId)
            throw new ConflictException("Not allowed");

        if (initiative.EndedAt != null)
            throw new ConflictException("Already ended");

        await FinalizeInitiativeAsync(initiativeId);
    }

    public async Task<List<UserDTO>> GetParticipantsAsync(Guid initiativeId, Guid userId)
{
    var initiative = await _database.Initiatives
        .FirstOrDefaultAsync(i => i.Id == initiativeId);

    if (initiative == null)
        throw new NotFoundException("Initiative not found");

    
    var participants = await _database.InitiativeParticipators 
        .Where(p => p.InitiativeId == initiativeId)
        .Select(p => p.User)
        .ToListAsync();

    return participants.Adapt<List<UserDTO>>();
}


    public async Task FinalizeInitiativeAsync(Guid initiativeId) //H�r ska po�ng ges till deltagare
    {
        var initiative = await _database.Initiatives
            .Include(i => i.InitiativeParticipators)
            .FirstOrDefaultAsync(i => i.Id == initiativeId);

        if (initiative == null)
        {
            _logger.LogError("Initiative not found. Terminating finalization.");
            return;
        }

        if (initiative.EndedAt != null)
        {
            _logger.LogError("Initiative has already ended. Terminating finalization.");
            return;
        }

        if (initiative.EcoPointsPerParticipant == 0)
        {
            _logger.LogError("Eco points per participant cannot be zero. Terminating finalization.");
            throw new ArgumentException("Eco points per participant cannot be zero.");
        }

        initiative.EndedAt = DateTime.UtcNow;
        await _database.SaveChangesAsync();

        var participants = await GetParticipantDataAsync(initiative);

        InitiativeEcoPointRequestDTO ecoPointRequest = new(
            initiativeId,
            participants,
            initiative.EcoPointsPerParticipant
        );

        await _ecoPointCommandInvoker.InvokeAsync(new AwardInitiativeEcoPointsCommand(ecoPointRequest));
    }

    public async Task<InitiativeDTO> JoinInitiativeAsync(Guid initiativeId, Guid userId)
    {
        var initiative = await _database.Initiatives
            .Include(i => i.InitiativeLikes)
            .Include(i => i.InitiativeParticipators)
            .FirstOrDefaultAsync(i => i.Id == initiativeId);
        
        if (initiative == null)
        {
            throw new NotFoundException("Initiative not found");
        }
        
        if (initiative.EndedAt != null)
        {
            throw new ConflictException("Initiative has already ended");
        }
        
        var initiativeParticipator = new InitiativeParticipator
        {
            InitiativeId = initiativeId,
            UserId = userId
        };
        
        await _database.InitiativeParticipators.AddAsync(initiativeParticipator);
        await _database.SaveChangesAsync();
        
        initiative.InitiativeParticipators.Add(initiativeParticipator);
        return ToInitiativeDto(initiative, userId);
    }
    
    public async Task LeaveInitiativeAsync(Guid initiativeId, Guid userId)
    {
        var initiativeParticipator = await _database.InitiativeParticipators
            .FirstOrDefaultAsync(i => i.InitiativeId == initiativeId && i.UserId == userId);
        
        if (initiativeParticipator == null)
        {
            throw new NotFoundException("Initiative participator not found");
        }
        
        _database.InitiativeParticipators.Remove(initiativeParticipator);
        await _database.SaveChangesAsync();
    }

    public async Task<InitiativeDTO> LikeInitiativeAsync(Guid initiativeId, Guid userId)
    {
        var initiative = await _database.Initiatives
            .Include(i => i.InitiativeLikes)
            .Include(i => i.InitiativeParticipators)
            .FirstOrDefaultAsync(i => i.Id == initiativeId);

        if (initiative == null)
            throw new NotFoundException("Initiative not found");

        if (!initiative.InitiativeLikes.Any(l => l.UserId == userId))
        {
            initiative.InitiativeLikes.Add(new InitiativeLike
            {
                InitiativeId = initiativeId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            });

            await _database.SaveChangesAsync();
        }

        return ToInitiativeDto(initiative, userId);
    }

    public async Task UnlikeInitiativeAsync(Guid initiativeId, Guid userId)
    {
        var like = await _database.InitiativeLikes
            .FirstOrDefaultAsync(l => l.InitiativeId == initiativeId && l.UserId == userId);

        if (like == null)
            return;

        _database.InitiativeLikes.Remove(like);
        await _database.SaveChangesAsync();
    }

    private async Task<Dictionary<Guid, Guid>> GetParticipantDataAsync(Initiative initiative)
    {
        var participantIds = initiative.InitiativeParticipators
            .Select(p => p.UserId)
            .ToList();

        return await _database.CommunityResidents
            .Where(cr => participantIds.Contains(cr.UserId))
            .ToDictionaryAsync(cr => cr.UserId, cr => cr.CommunityId);
    }

    private static InitiativeDTO ToInitiativeDto(Initiative initiative, Guid userId)
    {
        return initiative.Adapt<InitiativeDTO>() with
        {
            IsParticipating = initiative.InitiativeParticipators.Any(p => p.UserId == userId),
            ParticipantCount = initiative.InitiativeParticipators.Count,
            LikeCount = initiative.InitiativeLikes.Count,
            IsLiked = initiative.InitiativeLikes.Any(l => l.UserId == userId)
        };
    }
}
