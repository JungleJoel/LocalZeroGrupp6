using backend.Models;
using backend.Models.DTOs;
using backend.Models.DTOs.Requests;
using backend.Models.Entities;

namespace backend.Interfaces;

public interface IEcoPointService
{
    Task<EcoPointTransactionDTO> AwardEcoPointsUserAsync(EcoPointRequestDTO ecoPointRequestDTO);
    
    Task<EcoPointTransactionDTO> DeductEcoPointsUserAsync(EcoPointRequestDTO ecoPointRequestDTO);
    
    Task<EcoPointBalanceDTO> GetUserEcoPointBalanceAsync(Guid communityId, Guid userId);
    
    Task<List<EcoPointTransactionDTO>> GetUserEcoPointHistoryAsync(Guid communityId, Guid userId);
    
    Task<EcoPointBalanceDTO> GetCommunityEcoPointBalanceAsync(Guid communityId, Guid userId);

    Task<List<EcoPointTransactionDTO>> GetCommunityEcoPointHistoryAsync(Guid communityId, Guid userId);
    
    Task GetCommunityLeaderboardAsync(Guid communityId, Guid userId);
    
    Task<EcoPointTransaction> AwardInitiativeEcoPointsAsync(Initiative initiative);
}