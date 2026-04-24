using backend.Models;
using backend.Models.DTOs;


namespace backend.Interfaces;

public interface IEcoPointService : IEcoPointTransactions
{
    Task<EcoPointBalanceDTO> GetUserEcoPointBalanceAsync(Guid communityId, Guid userId);
    
    Task<List<EcoPointTransactionDTO>> GetUserEcoPointHistoryAsync(Guid communityId, Guid userId);
    
    Task<EcoPointBalanceDTO> GetCommunityEcoPointBalanceAsync(Guid communityId, Guid userId);

    Task<List<EcoPointTransactionDTO>> GetCommunityEcoPointHistoryAsync(Guid communityId, Guid userId);
    
    Task GetCommunityLeaderboardAsync(Guid communityId, Guid userId);
}