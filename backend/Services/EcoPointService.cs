using backend.Data;
using backend.Exceptions;
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
    public async Task<EcoPointTransactionDTO> AwardEcoPointsUserAsync(EcoPointRequestDTO ecoPointRequestDTO)
    {
        await EnsureResidentAsync(ecoPointRequestDTO.CommunityId, ecoPointRequestDTO.UserId);
        
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
        await database.SaveChangesAsync();
        
        return ecoPointTransaction.Adapt<EcoPointTransactionDTO>();
    }
    
    //HELT ÄRLIGT KAN VI NOG SLÅ IHOP DESSA PÅ NÅGOT SÄTT MEN JAG HAR INTE HUVUDET TILL ATT GÖRA DET
    public async Task<EcoPointTransactionDTO> DeductEcoPointsUserAsync(EcoPointRequestDTO ecoPointRequestDTO)
    {
        await EnsureResidentAsync(ecoPointRequestDTO.CommunityId, ecoPointRequestDTO.UserId);
        
        if(ecoPointRequestDTO.Amount <= 0)
            throw new ArgumentException("Amount must be greater than zero");
        
        var ecoPointTransaction = new EcoPointTransaction
        {
            Id = Guid.NewGuid(),
            CommunityId = ecoPointRequestDTO.CommunityId,
            UserId = ecoPointRequestDTO.UserId,
            InitiativeId = ecoPointRequestDTO.InitiativeId,
            Amount = -ecoPointRequestDTO.Amount, //HÄR ÄR SKILLNADEN!
            CreatedAt = DateTime.UtcNow
        };
        
        await database.EcoPointTransactions.AddAsync(ecoPointTransaction);
        await database.SaveChangesAsync();
        
        return ecoPointTransaction.Adapt<EcoPointTransactionDTO>();
    }
    
    public async Task<EcoPointBalanceDTO> GetUserEcoPointBalanceAsync(Guid communityId, Guid userId)
    {
        await EnsureResidentAsync(communityId, userId);
        
        var userEcoPointBalance = await database.EcoPointTransactions
            .Where(x => x.UserId == userId)
            .SumAsync(x => x.Amount);

        var userEcoPointBalanceDTO = new EcoPointBalanceDTO(userId, userEcoPointBalance);

        return userEcoPointBalanceDTO;
    }
    
    public async Task<List<EcoPointTransactionDTO>> GetUserEcoPointHistoryAsync(Guid communityId,  Guid userId)
    {
        await EnsureResidentAsync(communityId, userId);
        
        var ecoPointHistory = await database.EcoPointTransactions
            .Where(x => x.UserId == userId)
            .ToListAsync();
        
        return  ecoPointHistory.Adapt<List<EcoPointTransactionDTO>>();
    }
    
    public async Task<EcoPointBalanceDTO> GetCommunityEcoPointBalanceAsync(Guid communityId, Guid userId)
    {
        await EnsureResidentAsync(communityId, userId);
        
        var communityEcoPointBalance = await database.EcoPointTransactions.Where(x => x.CommunityId == communityId).SumAsync(x => x.Amount);

        var communityEcoPointBalanceDTO = new EcoPointBalanceDTO(communityId, communityEcoPointBalance);

        return communityEcoPointBalanceDTO;
    }

    public async Task<List<EcoPointTransactionDTO>> GetCommunityEcoPointHistoryAsync(Guid communityId, Guid userId)
    {
        await EnsureResidentAsync(communityId, userId);

        var ecoPointHistory = await database.EcoPointTransactions
            .Where(x => x.CommunityId == communityId)
            .ToListAsync();
        
        return ecoPointHistory.Adapt<List<EcoPointTransactionDTO>>();
    }

    //FINNS ATT IMPLEMENTERA OM MAN KÄNNER ATT MAN HAR VÄLDIGT TRÅKIGT EN REGNIG SÖNDAG
    public async Task GetCommunityLeaderboardAsync(Guid communityId, Guid userId)
    {
        throw new NotImplementedException();
    }
    
    public Task<EcoPointTransaction> AwardInitiativeEcoPointsAsync(Initiative initiative)
    {
        //WAITING FOR INITIATIVESERVICE TO BE IMPLEMENTED WALLA ONLY TO BE USED INSIDE BACKEND!!!
        throw new NotImplementedException();
    }
    
    private async Task EnsureResidentAsync(Guid communityId, Guid userId)
    {
        var isResident = await database.CommunityResidents.AnyAsync(x => x.UserId == userId && x.CommunityId == communityId);
        
        if(!isResident)
            throw new NotFoundException("Incorrect user or community");
    }
}