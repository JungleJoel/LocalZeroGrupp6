using backend.Interfaces;
using backend.Models;
using backend.Models.DTOs.Requests;

namespace backend.Services;

public record AwardInitiativeEcoPointsCommand(InitiativeEcoPointRequestDTO Request) : IEcoPointCommand<List<EcoPointTransactionDTO>>
{
    public Task<List<EcoPointTransactionDTO>> ExecuteAsync(IEcoPointTransactions receiver) 
        => receiver.AwardInitiativeEcoPointsAsync(Request);   
}