using backend.Interfaces;
using backend.Models;
using backend.Models.DTOs.Requests;

namespace backend.Services.EcoPoint;

public record AwardEcoPointsCommand(EcoPointRequestDTO Request) : IEcoPointCommand<EcoPointTransactionDTO>
{
    public Task<EcoPointTransactionDTO> ExecuteAsync(IEcoPointTransactions receiver) 
        => receiver.AwardEcoPointsUserAsync(Request);
}