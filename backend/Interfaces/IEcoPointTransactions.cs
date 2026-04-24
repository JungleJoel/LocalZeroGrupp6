using backend.Models;
using backend.Models.DTOs.Requests;
using backend.Models.Entities;

namespace backend.Interfaces;

public interface IEcoPointTransactions
{
    Task<EcoPointTransactionDTO> AwardEcoPointsUserAsync(EcoPointRequestDTO ecoPointRequestDTO);
    
    Task<EcoPointTransactionDTO> DeductEcoPointsUserAsync(EcoPointRequestDTO ecoPointRequestDTO);
    
    Task<List<EcoPointTransactionDTO>> AwardInitiativeEcoPointsAsync(InitiativeEcoPointRequestDTO initiativeEcoPointRequestDTO);
}