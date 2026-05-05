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

public class EcoPointService : IEcoPointService
{
    
    private readonly ApplicationDbContext _database;
    private readonly ICommunityValidationService _communityValidationService;

    public EcoPointService(ApplicationDbContext database, ICommunityValidationService communityValidationService)
    {
        _database = database;
        _communityValidationService = communityValidationService;
    }
    
    public async Task<EcoPointTransactionDTO> AwardEcoPointsUserAsync(EcoPointRequestDTO ecoPointRequestDTO)
    {
        return await CreateEcoPointTransactionAsync(ecoPointRequestDTO, ecoPointRequestDTO.Amount);
    }
    
    public async Task<EcoPointTransactionDTO> DeductEcoPointsUserAsync(EcoPointRequestDTO ecoPointRequestDTO)
    {
        return await CreateEcoPointTransactionAsync(ecoPointRequestDTO, -ecoPointRequestDTO.Amount);
    }
    
    public async Task<EcoPointBalanceDTO> GetUserEcoPointBalanceAsync(Guid communityId, Guid userId)
    {
        if (!await _communityValidationService.IsResidentInCommunityAsync(communityId, userId))
        {
            throw new NotFoundException("User is not a resident in this community");
        }
        
        var userEcoPointBalance = await _database.EcoPointTransactions
            .Where(x => x.UserId == userId)
            .SumAsync(x => x.Amount);

        var userEcoPointBalanceDTO = new EcoPointBalanceDTO(userId, userEcoPointBalance);

        return userEcoPointBalanceDTO;
    }
    
    public async Task<List<EcoPointTransactionDTO>> GetUserEcoPointHistoryAsync(Guid communityId,  Guid userId)
    {
        if (!await _communityValidationService.IsResidentInCommunityAsync(communityId, userId))
        {
            throw new NotFoundException("User is not a resident in this community");
        }
        
        var ecoPointHistory = await _database.EcoPointTransactions
            .Where(x => x.UserId == userId)
            .ToListAsync();
        
        return  ecoPointHistory.Adapt<List<EcoPointTransactionDTO>>();
    }
    
    public async Task<EcoPointBalanceDTO> GetCommunityEcoPointBalanceAsync(Guid communityId, Guid userId)
    {
        if (!await _communityValidationService.IsResidentInCommunityAsync(communityId, userId))
        {
            throw new NotFoundException("User is not a resident in this community");
        }
        
        var communityEcoPointBalance = await _database.EcoPointTransactions.Where(x => x.CommunityId == communityId).SumAsync(x => x.Amount);

        var communityEcoPointBalanceDTO = new EcoPointBalanceDTO(communityId, communityEcoPointBalance);

        return communityEcoPointBalanceDTO;
    }

    public async Task<List<EcoPointTransactionDTO>> GetCommunityEcoPointHistoryAsync(Guid communityId, Guid userId)
    {
        if (!await _communityValidationService.IsResidentInCommunityAsync(communityId, userId))
        {
            throw new NotFoundException("User is not a resident in this community");
        }

        var ecoPointHistory = await _database.EcoPointTransactions
            .Where(x => x.CommunityId == communityId)
            .ToListAsync();
        
        return ecoPointHistory.Adapt<List<EcoPointTransactionDTO>>();
    }

    //FINNS ATT IMPLEMENTERA OM MAN KÄNNER ATT MAN HAR VÄLDIGT TRÅKIGT EN REGNIG SÖNDAG
    public async Task GetCommunityLeaderboardAsync(Guid communityId, Guid userId)
    {
        throw new NotImplementedException();
    }
    
    public async Task<List<EcoPointTransactionDTO>> AwardInitiativeEcoPointsAsync(InitiativeEcoPointRequestDTO initiativeEcoPointRequestDTO)
    {
        var ecoPointTransactions = new List<EcoPointTransactionDTO>();

        foreach (var user in initiativeEcoPointRequestDTO.Users)
        {
            var ecoPointRequest = new EcoPointRequestDTO(
                user.Community!.Id,
                user.Id,
                initiativeEcoPointRequestDTO.InitiativeId,
                initiativeEcoPointRequestDTO.EcoPointAmount,
                initiativeEcoPointRequestDTO.Reason
            );
            
            var ecoPointTransaction = await AwardEcoPointsUserAsync(ecoPointRequest);
            ecoPointTransactions.Add(ecoPointTransaction);
        }
        
        return ecoPointTransactions;
    }

    private async Task<EcoPointTransactionDTO> CreateEcoPointTransactionAsync(EcoPointRequestDTO dto, int amount)
    {
        if (!await _communityValidationService.IsResidentInCommunityAsync(dto.CommunityId, dto.UserId))
        {
            throw new NotFoundException("User is not a resident in this community");
        }
        
        if(dto.Amount <= 0)
            throw new ArgumentException("Amount must be greater than zero");
        
        var ecoPointTransaction = new EcoPointTransaction
        {
            Id = Guid.NewGuid(),
            CommunityId = dto.CommunityId,
            UserId = dto.UserId,
            InitiativeId = dto.InitiativeId,
            Amount = amount, 
            CreatedAt = DateTime.UtcNow,
            Reason = dto.Reason
        };
        
        await _database.EcoPointTransactions.AddAsync(ecoPointTransaction);
        await _database.SaveChangesAsync();
        
        return ecoPointTransaction.Adapt<EcoPointTransactionDTO>();
    }
}