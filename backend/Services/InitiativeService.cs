using backend.Data;
using backend.Models.DTOs;
using backend.Models.DTOs.Requests;
using backend.Models.Entities;
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

        return initiative.Adapt<InitiativeDTO>();
    }

    public async Task<List<InitiativeDTO>> GetInitiativesAsync()
    {
        return (await _database.Initiatives.ToListAsync())
            .Adapt<List<InitiativeDTO>>();
    }

    public async Task<InitiativeDTO> GetInitiativeAsync(Guid initiativeId, Guid userId)
    {
        var result = await _database.Initiatives
            .Where(i => i.Id == initiativeId)
            .Select(i => new
            {
                Initiative = i,
                IsParticipating = i.InitiativeParticipators.Any(p => p.UserId == userId)
            })
            .FirstOrDefaultAsync();

        if (result is null)
            throw new NotFoundException("Initiative not found");

        return result.Initiative.Adapt<InitiativeDTO>() with { IsParticipating = result.IsParticipating };
    }

    public async Task CancelInitiativeAsync(Guid inititativeId, Guid userId)
    {
        var initiative = await _database.Initiatives
            .FirstOrDefaultAsync(i => i.Id == inititativeId);

        if (initiative == null)
            throw new NotFoundException("Initiative not found");

        if (initiative.CreatedBy != userId)
            throw new ConflictException("Not allowed");

        _database.Initiatives.Remove(initiative);

        await _database.SaveChangesAsync();
    }

    public async Task<List<InitiativeDTO>> GetByCommunityIdAsync(Guid communityId, Guid userId)
    {
        var results = await _database.Initiatives
            .Where(i => i.CommunityId == communityId && i.EndedAt == null)
            .OrderBy(i => i.StartsAt)
            .Select(i => new
            {
                Initiative = i,
                IsParticipating = i.InitiativeParticipators.Any(p => p.UserId == userId)
            })
            .ToListAsync();

        return results
            .Select(x => x.Initiative.Adapt<InitiativeDTO>() with { IsParticipating = x.IsParticipating })
            .ToList();
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

        List<UserDTO> users = await GetUsersFromInitiativeAsync(initiative);

        InitiativeEcoPointRequestDTO ecoPointRequest = new InitiativeEcoPointRequestDTO(
            initiativeId,
            users,
            initiative.EcoPointsPerParticipant
        );

        await _ecoPointCommandInvoker.InvokeAsync(new AwardInitiativeEcoPointsCommand(ecoPointRequest));
    }

    public async Task<InitiativeDTO> JoinInitiativeAsync(Guid initiativeId, Guid userId)
    {
        var initiative = await _database.Initiatives
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
        
        return initiative.Adapt<InitiativeDTO>();
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

    private async Task<List<UserDTO>> GetUsersFromInitiativeAsync(Initiative initiative)
    {
        List<UserDTO> users = new List<UserDTO>();

        foreach (var participant in initiative.InitiativeParticipators)
        {
            var user = await _database.FindAsync<User>(participant.UserId);
            if (user != null)
            {
                var userDto = user.Adapt<UserDTO>();
                users.Add(userDto);

            }
        }

        return users;
    }
}