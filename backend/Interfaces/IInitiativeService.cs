using backend.Models.DTOs;
using backend.Models.DTOs.Requests;

namespace backend.Interfaces;

public interface IInitiativeService
{
    Task<InitiativeDTO> CreateInitiativeAsync(Guid userId, CreateInitiativeRequestDTO request);

    Task<List<InitiativeDTO>> GetInitiativesAsync(Guid userId);

    Task<InitiativeDTO> GetInitiativeAsync(Guid initiativeId, Guid userId);

    Task RemoveInitiativeAsync(Guid initiativeId, Guid userId);

    Task EndInitiativeAsync(Guid initiativeId, Guid userId);

    Task<List<InitiativeDTO>> GetByCommunityIdAsync(Guid communityId, Guid userId);

    Task FinalizeInitiativeAsync(Guid initiativeId);

    Task<InitiativeDTO> JoinInitiativeAsync(Guid initiativeId, Guid userId);

    Task LeaveInitiativeAsync(Guid initiativeId, Guid userId);

    Task<InitiativeDTO> LikeInitiativeAsync(Guid initiativeId, Guid userId);

    Task UnlikeInitiativeAsync(Guid initiativeId, Guid userId);

    Task<List<UserDTO>> GetParticipantsAsync(Guid initiativeId, Guid userId);
}
