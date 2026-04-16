using backend.Models.DTOs;
using backend.Models.DTOs.Requests;

namespace backend.Interfaces;

public interface ICommunityService
{
    Task<List<CommunityDTO>> GetCommunitiesAsync();
    
    Task<CommunityDTO> GetCommunityAsync(Guid id);
    
    Task<CommunityJoinRequestDTO> SubmitJoinRequestAsync(Guid userId, Guid communityId);
    
    Task LeaveCommunityAsync(Guid userId, Guid communityId);
    
}