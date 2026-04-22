using backend.Data;
using backend.Interfaces;
using backend.Models;
using backend.Models.DTOs;
using backend.Models.DTOs.Requests;
using backend.Models.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class EcoPointService(ApplicationDbContext database) : IEcoPointService
{
    public async Task<EcoPointTransactionDTO> AwardPointsUserAsync(EcoPointRequestDTO ecoPointRequestDTO)
    {
        if(ecoPointRequestDTO.Amount <= 0)
            throw new ArgumentException("Amount must be greater than zero");
        
        var ecoPointTransaction = new EcoPointTransaction
        {
            Id = Guid.NewGuid(),
            CommunityId = ecoPointRequestDTO.CommunityId,
            UserId = ecoPointRequestDTO.UserId,
            InitiativeId = ecoPointRequestDTO.InitiativeId,
            Amount = ecoPointRequestDTO.Amount, 
            CreatedAt = DateTime.UtcNow
        };
        
        await database.EcoPointTransactions.AddAsync(ecoPointTransaction);
        
        return ecoPointTransaction.Adapt<EcoPointTransactionDTO>();
    }

    public async Task<EcoPointTransactionDTO> DeductPointsUserAsync(EcoPointRequestDTO ecoPointRequestDTO)
    {
        if(ecoPointRequestDTO.Amount >= 0)
            throw new ArgumentException("Amount must be less than zero");
        
        var ecoPointTransaction = new EcoPointTransaction
        {
            Id = Guid.NewGuid(),
            CommunityId = ecoPointRequestDTO.CommunityId,
            UserId = ecoPointRequestDTO.UserId,
            InitiativeId = ecoPointRequestDTO.InitiativeId,
            Amount = -ecoPointRequestDTO.Amount,
            CreatedAt = DateTime.UtcNow
        };
        
        await database.EcoPointTransactions.AddAsync(ecoPointTransaction);
        
        return ecoPointTransaction.Adapt<EcoPointTransactionDTO>();
    }

    public Task<EcoPointTransaction> AwardInitiativePointsAsync(Initiative initiative)
    {
        //WAITING FOR INITIATIVESERVICE TO BE IMPLEMENTED WALLA
        throw new NotImplementedException();
    }

    public async Task<UserEcoPointBalanceDTO> GetUserPointBalanceAsync(Guid userId)
    {
        var userEcoPointBalance = await database.EcoPointTransactions.Where(x => x.UserId == userId).SumAsync(x => x.Amount);

        var userEcoPointBalanceDTO = new UserEcoPointBalanceDTO(userId, userEcoPointBalance);

        return userEcoPointBalanceDTO;
    }

    public async Task<CommunityEcoPointBalanceDTO> GetCommunityPointBalanceAsync(Guid communityId)
    {
        
        var communityEcoPointBalance = await database.EcoPointTransactions.Where(x => x.CommunityId == communityId).SumAsync(x => x.Amount);

        var communityEcoPointBalanceDTO = new CommunityEcoPointBalanceDTO(communityId, communityEcoPointBalance);

        return communityEcoPointBalanceDTO;
    }
}