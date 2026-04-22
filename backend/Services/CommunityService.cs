using backend.Data;
using backend.Exceptions;
using backend.Interfaces;
using backend.Models.DTOs;
using backend.Models.DTOs.Requests;
using backend.Models.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class CommunityService(ApplicationDbContext database) : ICommunityService
{
    public async Task<List<CommunityDTO>> GetCommunitiesAsync()
    {
        List<Community> communities = await database.Communities.ToListAsync();
        return communities.Adapt<List<CommunityDTO>>();
    }

    public async Task<CommunityDTO> GetCommunityAsync(Guid id)
    {
        var community = await database.FindAsync<Community>(id);

        if (community == null)
        {
            throw new KeyNotFoundException($"Community with id {id} not found");
        }
        
        return community.Adapt<CommunityDTO>();
    }

    public async Task<CommunityJoinRequestDTO> SubmitJoinRequestAsync(Guid userId, Guid communityId)
    {
        
        var isAlreadyResident = await database.CommunityResidents
            .AnyAsync(resident => resident.UserId == userId);

        if (isAlreadyResident)
            throw new ConflictException("User is already a member in community");

        var existingRequest = await database.CommunityJoinRequests
            .AnyAsync(resident => resident.UserId == userId && resident.CommunityId == communityId && resident.IsAccepted == null);
        
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
        
        await database.CommunityJoinRequests.AddAsync(joinRequest);
        await database.SaveChangesAsync();
        
        return joinRequest.Adapt<CommunityJoinRequestDTO>();
    }

    public async Task<List<CommunityJoinRequestDTO>> GetRequestsAsync(Guid managerUserId, Guid communityId)
    {
        bool isManager = await IsManagerAsync(managerUserId, communityId);

        if (!isManager)
        {
            throw new ConflictException("Not a manager over this community");
        }
        
        var requests = await  database.CommunityJoinRequests
            .Where(resident => resident.CommunityId == communityId)
            .ToListAsync();
        
        return requests.Adapt<List<CommunityJoinRequestDTO>>();
    }

    public async Task<CommunityJoinRequestDTO> ApproveRequestAsync(Guid requestId, Guid managerUserId, Guid communityId)
    {
        
        bool isManager = await IsManagerAsync(managerUserId, communityId);

        if (!isManager)
        {
            throw new ConflictException("Not a manager over this community");
        }
        
        var request = await database.CommunityJoinRequests
            .FirstOrDefaultAsync(request => request.Id == requestId && request.CommunityId == communityId);
        
        if(request == null)
            throw new NotFoundException("Request not found");
        
        if(request.IsAccepted != null)
            throw new ConflictException("Request has already been reviewed");
        
        var alreadyMember = await database.CommunityResidents
                    .AnyAsync(resident => resident.UserId == request.UserId && resident.CommunityId == request.CommunityId);
                
        if(alreadyMember)
            throw new ConflictException("User is already a member in community");
        
        await using var transaction = await database.Database.BeginTransactionAsync();
        
        request.IsAccepted = true;
        request.ReviewedBy = managerUserId;
        
        database.CommunityJoinRequests.Update(request);

        database.CommunityResidents.Add(new CommunityResident
        {
            CommunityId = request.CommunityId,
            UserId = request.UserId,
            IsManager = false,
            CreatedAt = DateTime.UtcNow
        });
        
        await database.SaveChangesAsync();
        await transaction.CommitAsync();
        
        return request.Adapt<CommunityJoinRequestDTO>();
    }

    public async Task<CommunityJoinRequestDTO> DeclineRequestAsync(Guid requestId, Guid managerUserId, Guid communityId)
    {
        
        bool isManager = await IsManagerAsync(managerUserId, communityId);

        if (!isManager)
        {
            throw new ConflictException("Not a manager over this community");
        }
        
        var request = await database.CommunityJoinRequests
            .FirstOrDefaultAsync(request => request.Id == requestId && request.CommunityId == communityId);

        if(request == null)
            throw new NotFoundException("Request not found");
        
        if(request.IsAccepted != null)
            throw new ConflictException("Request has already been reviewed");
        
        request.IsAccepted = false;
        request.ReviewedBy = managerUserId;
        
        database.CommunityJoinRequests.Update(request);
        await database.SaveChangesAsync();
        
        return request.Adapt<CommunityJoinRequestDTO>();
    }
    
    public async Task LeaveCommunityAsync(Guid userId, Guid communityId)
    {
        await using var transaction = await database.Database.BeginTransactionAsync();

        var communityResident = await database.CommunityResidents
            .FirstOrDefaultAsync(resident => resident.UserId == userId && resident.CommunityId == communityId);

        if (communityResident == null)
            throw new NotFoundException("User is not a resident in a community");

        if (communityResident.IsManager)
        {
            var managerCount = await CountManagersInCommunityAsync(communityId);

            if (managerCount < 2)
                throw new ConflictException("There must be at least one manager in a community");
        }

        database.CommunityResidents.Remove(communityResident);

        await database.SaveChangesAsync();
        await transaction.CommitAsync();
    }
    
    
    public async Task<List<CommunityMembershipDTO>> GetUserCommunityAsync(Guid userId)
    {
        return await database.CommunityResidents
            .Where(resident => resident.UserId == userId)
            .Select(r => new CommunityMembershipDTO
            {
                CommunityId = r.CommunityId,
                CommunityName = r.Community.Name,
                IsManager = r.IsManager
            })
            .ToListAsync();
    }
    
    public async Task<bool> IsManagerAsync(Guid userId, Guid communityId)
    {
        return await database.CommunityResidents
            .AnyAsync(resident => resident.UserId == userId && resident.CommunityId == communityId && resident.IsManager == true);
    }

    private async Task<int> CountManagersInCommunityAsync(Guid communityId)
    {
        return await database.CommunityResidents.CountAsync(resident =>
            resident.CommunityId == communityId && resident.IsManager);
    }
    
}