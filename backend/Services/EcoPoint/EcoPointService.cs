using backend.Data;
using backend.Exceptions;
using backend.Interfaces;
using backend.Models;
using backend.Models.DTOs;
using backend.Models.DTOs.Requests;
using backend.Models.Entities;
using backend.Settings;
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
        if (ecoPointRequestDTO.InitiativeId == null &&
            ecoPointRequestDTO.Amount > EcoPointsConfig.Instance().MaxEcoPointsPerAction)
            throw new ArgumentException($"Eco points per action cannot exceed {EcoPointsConfig.Instance().MaxEcoPointsPerAction}.");

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
    
    public async Task<List<EcoPointTransactionDTO>> GetEcoActionHistoryAsync(Guid communityId, Guid userId)
    {
        if (!await _communityValidationService.IsResidentInCommunityAsync(communityId, userId))
        {
            throw new NotFoundException("User is not a resident in this community");
        }

        var ecoPointTransactions = await _database.EcoPointTransactions
            .Where(transaction =>
                transaction.CommunityId == communityId &&
                transaction.UserId == userId &&
                transaction.InitiativeId == null)
            .OrderByDescending(transaction => transaction.CreatedAt)
            .ToListAsync();

        return ecoPointTransactions.Adapt<List<EcoPointTransactionDTO>>();
    }

    public async Task<CommunityEcoActionSummaryDTO> GetCommunityEcoActionSummaryAsync(Guid communityId, Guid requestingUserId)
    {
        if (!await _communityValidationService.IsResidentInCommunityAsync(communityId, requestingUserId))
        {
            throw new NotFoundException("User is not a resident in this community");
        }

        var ecoActions = _database.EcoPointTransactions
            .Where(transaction => transaction.CommunityId == communityId && transaction.InitiativeId == null);
        var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var topAction = await ecoActions
            .Where(transaction => transaction.Reason != null && transaction.Reason != "")
            .GroupBy(transaction => transaction.Reason)
            .Select(group => new { Reason = group.Key, Count = group.Count() })
            .OrderByDescending(group => group.Count)
            .FirstOrDefaultAsync();

        return new CommunityEcoActionSummaryDTO(
            CommunityId: communityId,
            TotalActions: await ecoActions.CountAsync(),
            TotalEcoPoints: await ecoActions.SumAsync(transaction => (int?)transaction.Amount) ?? 0,
            ActiveMembers: await ecoActions.Select(transaction => transaction.UserId).Distinct().CountAsync(),
            ActionsThisMonth: await ecoActions.CountAsync(transaction => transaction.CreatedAt >= monthStart),
            TopActionReason: topAction?.Reason,
            TopActionCount: topAction?.Count ?? 0
        );
    }
    
    public async Task<List<EcoPointTransactionDTO>> AwardInitiativeEcoPointsAsync(InitiativeEcoPointRequestDTO initiativeEcoPointRequestDTO)
    {
        var ecoPointTransactions = new List<EcoPointTransactionDTO>();

        foreach (var (userId, communityId) in initiativeEcoPointRequestDTO.Participants)
        {
            var ecoPointRequest = new EcoPointRequestDTO(
                communityId,
                userId,
                initiativeEcoPointRequestDTO.InitiativeId,
                initiativeEcoPointRequestDTO.EcoPointAmount,
                Reason: null
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

        if (dto.Amount > EcoPointsConfig.Instance().MaxEcoPointsPerParticipant)
            throw new ArgumentException($"A single transaction cannot exceed {EcoPointsConfig.Instance().MaxEcoPointsPerParticipant} eco points.");
        
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