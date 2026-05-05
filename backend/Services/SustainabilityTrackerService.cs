using backend.Data;
using backend.Interfaces;
using backend.Models;
using backend.Models.DTOs.Requests;
using Mapster;
using Microsoft.EntityFrameworkCore;


namespace backend.Services;

public class SustainabilityTrackerService : ISustainabilityTrackerService
{

    private readonly IEcoPointTransactions _ecoPointTransactions;
    
    private readonly ApplicationDbContext _database;

    public SustainabilityTrackerService(IEcoPointTransactions ecoPointTransactions, ApplicationDbContext database)
    {
        _ecoPointTransactions = ecoPointTransactions;
        _database = database;
    }

    public async Task<EcoPointTransactionDTO> AwardSustainability(EcoPointRequestDTO ecoPointRequestDTO)
    {
        return await _ecoPointTransactions.AwardEcoPointsUserAsync(ecoPointRequestDTO);
    }

    public async Task<List<EcoPointTransactionDTO>> GetSustainabilityTrackerHistory(Guid userId)
    {
        var ecoPointTransactions=  await _database.EcoPointTransactions
            .Where(user => user.InitiativeId == null)
            .ToListAsync();
        return ecoPointTransactions.Adapt<List<EcoPointTransactionDTO>>();
    }

}