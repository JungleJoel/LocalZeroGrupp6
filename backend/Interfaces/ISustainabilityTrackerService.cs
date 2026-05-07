using backend.Models;
using backend.Models.DTOs.Requests;

namespace backend.Interfaces;

public interface ISustainabilityTrackerService
{
    Task<EcoPointTransactionDTO> AwardSustainability(EcoPointRequestDTO ecoPointRequestDTO);
    Task<List<EcoPointTransactionDTO>> GetSustainabilityTrackerHistory(Guid userId);
}