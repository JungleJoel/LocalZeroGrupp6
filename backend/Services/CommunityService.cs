using backend.Data;
using backend.Exceptions;
using backend.Interfaces;
using backend.Models.DTOs;
using backend.Models.DTOs.Requests;
using backend.Models.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class CommunityService : ICommunityService
{
    
    private readonly ApplicationDbContext _database;

    public CommunityService(ApplicationDbContext database)
    {
        _database = database;
    }
    
    public async Task<List<CommunityDTO>> GetCommunitiesAsync()
    {
        List<Community> communities = await _database.Communities.ToListAsync();
        return communities.Adapt<List<CommunityDTO>>();
    }

    public async Task<CommunityDTO> GetCommunityAsync(Guid id)
    {
        var community = await _database.FindAsync<Community>(id);

        if (community == null)
        {
            throw new KeyNotFoundException($"Community with id {id} not found");
        }
        
        return community.Adapt<CommunityDTO>();
    }

    public async Task<CommunityJoinRequestDTO> SubmitJoinRequestAsync(Guid userId, Guid communityId)
    {
        var community = await GetCommunityAsync(communityId)
            ?? throw new NotFoundException("Community not found");

        var isAlreadyResident = await _database.CommunityResidents
            .AnyAsync(r => r.UserId == userId);

        if (isAlreadyResident)
            throw new ConflictException("User is already a member in community");

        var existingRequest = await _database.CommunityJoinRequests
            .AnyAsync(r => r.UserId == userId && r.IsAccepted == null);
        
        if(existingRequest)
            throw new ConflictException("User already have a pending join request for this community");
        
        var joinRequest = new CommunityJoinRequest
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CommunityId = communityId,
            IsAccepted = null,
            CreatedAt = DateTime.UtcNow
        };
        
        await _database.CommunityJoinRequests.AddAsync(joinRequest);
        await _database.SaveChangesAsync();
        
        return joinRequest.Adapt<CommunityJoinRequestDTO>();
    }
 
    public async Task LeaveCommunityAsync(Guid userId, Guid communityId)
    {
        var communityResident = await _database.CommunityResidents.FindAsync(userId);
        
        if(communityResident == null)
            throw new NotFoundException("User is not a resident in a community");
        
        if(communityResident.CommunityId != communityId)
            throw new ConflictException("User is a member in another community");

        if (communityResident.UserId == userId && communityResident.CommunityId == communityId)
        {
            _database.CommunityResidents.Remove(communityResident);
            await _database.SaveChangesAsync();
        }
    }
}