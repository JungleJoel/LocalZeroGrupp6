using backend.Data;
using backend.Exceptions;
using backend.Interfaces;
using backend.Models.DTOs;
using backend.Models.DTOs.Requests;
using backend.Models.DTOs.Responses;
using backend.Models.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class CommunityService : ICommunityService, ICommunityValidationService
{   
    private readonly ApplicationDbContext _database;
    private readonly IUserService _userService;

    public CommunityService(ApplicationDbContext database, IUserService userService)
    {
        _database = database;
        _userService = userService;
    }
    
    public async Task<List<CommunityDTO>> GetCommunitiesAsync()
    {
        List<Community> communities = await _database.Communities
            .Include(c => c.EcoPointTransactions)
            .Include(c => c.CommunityResidents)
            .AsSplitQuery()
            .ToListAsync();
        return communities.Adapt<List<CommunityDTO>>();
    }

    public async Task<CommunityDTO> GetCommunityAsync(Guid id)
    {
        var community = await _database.Communities
            .Include(c => c.EcoPointTransactions)
            .Include(c => c.CommunityResidents)
            .AsSplitQuery()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (community == null)
        {
            throw new KeyNotFoundException($"Community with id {id} not found");
        }
        
        return community.Adapt<CommunityDTO>();
    }

    public async Task<CommunityJoinRequestDTO> SubmitJoinRequestAsync(Guid userId, Guid communityId)
    {

        var isAlreadyResident = await IsResidentInCommunityAsync(communityId, userId);

        if (isAlreadyResident)
            throw new ConflictException("User is already a member in community");

        var existingRequest = await _database.CommunityJoinRequests
            .AnyAsync(r => r.UserId == userId && r.IsAccepted == null);

        if (existingRequest)
            throw new ConflictException("User already has a pending join request");
        
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

    public async Task<MyJoinRequestDTO?> GetMyJoinRequestAsync(Guid userId)
    {
        var request = await _database.CommunityJoinRequests
            .Include(r => r.Community)
            .FirstOrDefaultAsync(r => r.UserId == userId && r.IsAccepted == null);

        if (request == null) return null;

        return new MyJoinRequestDTO(request.Id, request.CommunityId, request.Community.Name, request.CreatedAt);
    }

    public async Task CancelJoinRequestAsync(Guid userId, Guid communityId)
    {
        var request = await _database.CommunityJoinRequests
            .FirstOrDefaultAsync(r => r.UserId == userId && r.CommunityId == communityId && r.IsAccepted == null);

        if (request == null)
            throw new NotFoundException("No pending join request found");

        _database.CommunityJoinRequests.Remove(request);
        await _database.SaveChangesAsync();
    }

    public async Task<List<CommunityJoinRequestWithUserDTO>> GetRequestsAsync(Guid managerUserId, Guid communityId)
    {
        bool isManager = await IsManagerAsync(managerUserId, communityId);

        if (!isManager)
            throw new ConflictException("Not a manager over this community");

        var requests = await _database.CommunityJoinRequests
            .Include(r => r.User)
            .Where(r => r.CommunityId == communityId && r.IsAccepted == null)
            .ToListAsync();

        return requests.Select(r => new CommunityJoinRequestWithUserDTO(
            r.Id, r.UserId, r.User.FirstName, r.User.LastName, r.CommunityId, r.IsAccepted, r.CreatedAt
        )).ToList();
    }

    public async Task<CommunityJoinRequestDTO> ApproveRequestAsync(Guid requestId, Guid managerUserId, Guid communityId)
    {
        
        bool isManager = await IsManagerAsync(managerUserId, communityId);

        if (!isManager)
        {
            throw new ConflictException("Not a manager over this community");
        }
        
        var request = await _database.CommunityJoinRequests
            .FirstOrDefaultAsync(request => request.Id == requestId && request.CommunityId == communityId);
        
        if(request == null)
            throw new NotFoundException("Request not found");
        
        if(request.IsAccepted != null)
            throw new ConflictException("Request has already been reviewed");
        
        var alreadyMember = await IsResidentInCommunityAsync(request.CommunityId, request.UserId);
                
        if(alreadyMember)
            throw new ConflictException("User is already a member in community");
        
        await using var transaction = await _database.Database.BeginTransactionAsync();
        
        request.IsAccepted = true;
        request.ReviewedBy = managerUserId;
        
        _database.CommunityJoinRequests.Update(request);

        _database.CommunityResidents.Add(new CommunityResident
        {
            CommunityId = request.CommunityId,
            UserId = request.UserId,
            IsManager = false,
            CreatedAt = DateTime.UtcNow
        });
        
        await _database.SaveChangesAsync();
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
        
        var request = await _database.CommunityJoinRequests
            .FirstOrDefaultAsync(request => request.Id == requestId && request.CommunityId == communityId);

        if(request == null)
            throw new NotFoundException("Request not found");
        
        if(request.IsAccepted != null)
            throw new ConflictException("Request has already been reviewed");
        
        request.IsAccepted = false;
        request.ReviewedBy = managerUserId;
        
        _database.CommunityJoinRequests.Update(request);
        await _database.SaveChangesAsync();
        
        return request.Adapt<CommunityJoinRequestDTO>();
    }
    
    public async Task LeaveCommunityAsync(Guid userId, Guid communityId)
    {
        await using var transaction = await _database.Database.BeginTransactionAsync();

        var communityResident = await _database.CommunityResidents
            .FirstOrDefaultAsync(resident => resident.UserId == userId && resident.CommunityId == communityId);

        if (communityResident == null)
            throw new NotFoundException("User is not a resident in a community");

        if (communityResident.IsManager)
        {
            var managerCount = await CountManagersInCommunityAsync(communityId);

            if (managerCount < 2)
                throw new ConflictException("There must be at least one manager in a community");
        }

        _database.CommunityResidents.Remove(communityResident);

        await _database.SaveChangesAsync();
        await transaction.CommitAsync();
    }


    public async Task<GetMyCommunityResponseDTO> GetMyCommunityAsync(Guid userId)
    {
        var user = await _userService.GetAsync(userId);

        if (user.Community == null)
        {
            throw new NotFoundException("User is not a member of any community.");
        }

        var community = await GetCommunityAsync(user.Community.Id);

        return new GetMyCommunityResponseDTO(
            community,
            user.IsCommunityManager ?? false
        );
    }
    
    public async Task<List<CommunityMemberDTO>> GetMembersAsync(Guid communityId, Guid requestingUserId)
    {
        var isResident = await IsResidentInCommunityAsync(communityId, requestingUserId);
        if (!isResident)
            throw new ForbiddenException("Not a member of this community");

        var residents = await _database.CommunityResidents
            .Include(r => r.User)
            .Where(r => r.CommunityId == communityId)
            .OrderByDescending(r => r.IsManager)
            .ThenBy(r => r.User.FirstName)
            .ToListAsync();

        return residents.Select(r => new CommunityMemberDTO(
            r.UserId, r.User.FirstName, r.User.LastName, r.User.AvatarImageUrl, r.IsManager, r.CreatedAt
        )).ToList();
    }

    public async Task<bool> IsResidentInCommunityAsync(Guid communityId, Guid userId)
    {
        return await _database.CommunityResidents.AnyAsync(x => x.UserId == userId && x.CommunityId == communityId);
    }

    public async Task<bool> IsManagerAsync(Guid userId, Guid communityId)
    {
        return await _database.CommunityResidents
            .AnyAsync(resident => resident.UserId == userId && resident.CommunityId == communityId && resident.IsManager == true);
    }

    private async Task<int> CountManagersInCommunityAsync(Guid communityId)
    {
        return await _database.CommunityResidents.CountAsync(resident =>
            resident.CommunityId == communityId && resident.IsManager);
    }
    
}