using backend.Models;
using backend.Models.DTOs.Requests;

namespace backend.Interfaces;

public interface IEcoActionsService
{
    Task<EcoPointTransactionDTO> AwardEcoAction(EcoPointRequestDTO ecoPointRequestDTO);
    Task<List<EcoPointTransactionDTO>> GetEcoActionHistory(Guid communityId, Guid userId);
    Task<CommunityEcoActionSummaryDTO> GetCommunityEcoActionSummary(Guid communityId, Guid requestingUserId);
}
