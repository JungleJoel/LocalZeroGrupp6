using backend.Data;
using backend.Interfaces;
using backend.Models;
using backend.Models.DTOs.Requests;


namespace backend.Services;

public class SustainabilityTrackerService : ISustainabilityTrackerService
{

    private readonly IEcoPointTransactions _ecoPointTransactions;
    
    private readonly IInitiativeService _initiativeService;
    
    private readonly ApplicationDbContext _database;

    public SustainabilityTrackerService(IEcoPointTransactions ecoPointTransactions, IInitiativeService initiativeService, ApplicationDbContext database)
    {
        _ecoPointTransactions = ecoPointTransactions;
        _initiativeService = initiativeService;
        _database = database;
    }

    public async Task<EcoPointTransactionDTO> AwardSustainability(Guid userId, CreateInitiativeRequestDTO createInitiativeRequestDto)
    {
       var newInitiative = await _initiativeService.CreateInitiativeAsync(userId, createInitiativeRequestDto);

       return await _ecoPointTransactions.AwardEcoPointsUserAsync(new EcoPointRequestDTO(
           CommunityId: newInitiative.CommunityId,
           UserId: userId,
           InitiativeId: newInitiative.Id,
           Amount: newInitiative.EcoPointsPerParticipant
       ));
    }

    public async Task GetSustainabilityTrackerHistory(Guid userId)
    {
        throw new NotImplementedException();
    }

}