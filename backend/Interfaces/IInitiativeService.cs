using backend.Models.DTOs;
using backend.Models.DTOs.Requests;

namespace backend.Interfaces;

public interface IInitiativeService
{
    Task<InitiativeDTO> CreateInitiativeAsync(Guid userId, CreateInitiativeRequestDTO request);

    Task<List<InitiativeDTO>> GetInitiativesAsync();

    Task<InitiativeDTO> GetInitiative(Guid id);

    Task CancelInitiativeAsync(Guid id, Guid userId);

    Task EndInitiativeAsync(Guid id, Guid userId);

    Task<List<InitiativeDTO>> GetByCommunityIdAsync(Guid communityId);
}