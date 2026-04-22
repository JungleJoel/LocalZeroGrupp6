using backend.Models;
using backend.Models.DTOs;
using backend.Models.DTOs.Requests;
using backend.Models.Entities;

namespace backend.Interfaces;

public interface IEcoPointService
{
    Task<EcoPointTransactionDTO> AwardPointsUserAsync(EcoPointRequestDTO ecoPointRequestDTO);
    
    Task<EcoPointTransactionDTO> DeductPointsUserAsync(EcoPointRequestDTO ecoPointRequestDTO);
    
    Task<EcoPointTransaction> AwardInitiativePointsAsync(Initiative initiative);
    
    Task<UserEcoPointBalanceDTO> GetUserPointBalanceAsync(Guid userId);
    
    Task<CommunityEcoPointBalanceDTO> GetCommunityPointBalanceAsync(Guid communityId);
    
}