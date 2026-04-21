using backend.Models;

namespace backend.Interfaces;

public interface IEcoPointService
{
    Task<EcoPointTransactionDTO> AwardPointsMemberAsync(Guid communityId, Guid userId, Guid initiativeId, int amount);
    
    Task<EcoPointTransactionDTO> DeductPointsMemberAsync(Guid communityId, Guid userId, Guid initiativeId, int amount);
    
    Task GetPointBalanceUserAsync(Guid communityId, Guid userId);
    
    Task GetPointBalanceCommunityAsync(Guid communityId);
    
    
}