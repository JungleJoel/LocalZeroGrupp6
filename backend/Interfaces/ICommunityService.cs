using backend.Models;
using backend.Models.DTOs;
using backend.Models.DTOs.Requests;
using backend.Models.DTOs.Responses;

namespace backend.Interfaces;

public interface ICommunityService
{
    Task<List<CommunityDTO>> GetCommunitiesAsync();
    
    Task<CommunityDTO> GetCommunityAsync(Guid id);
    
    Task<CommunityJoinRequestDTO> SubmitJoinRequestAsync(Guid userId, Guid communityId);
    
    Task<List<CommunityJoinRequestDTO>> GetRequestsAsync(Guid managerUserId, Guid communityId);
    
    Task<CommunityJoinRequestDTO> ApproveRequestAsync(Guid requestId, Guid managerUserId, Guid communityId);
    
    Task<CommunityJoinRequestDTO> DeclineRequestAsync(Guid requestId, Guid managerUserId, Guid communityId);
    
    Task LeaveCommunityAsync(Guid userId, Guid communityId);

    Task<bool> IsManagerAsync(Guid userId, Guid communityId);

    Task<GetMyCommunityResponseDTO> GetMyCommunityAsync(Guid userId);
}