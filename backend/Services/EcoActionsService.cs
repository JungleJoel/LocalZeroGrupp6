using backend.Data;
using backend.Exceptions;
using backend.Interfaces;
using backend.Models;
using backend.Models.DTOs.Requests;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class EcoActionsService : IEcoActionsService
{
    private readonly IEcoPointTransactions _ecoPointTransactions;
    private readonly ApplicationDbContext _database;
    private readonly ICommunityValidationService _communityValidationService;

    public EcoActionsService(
        IEcoPointTransactions ecoPointTransactions,
        ApplicationDbContext database,
        ICommunityValidationService communityValidationService)
    {
        _ecoPointTransactions = ecoPointTransactions;
        _database = database;
        _communityValidationService = communityValidationService;
    }

    public async Task<EcoPointTransactionDTO> AwardEcoAction(EcoPointRequestDTO ecoPointRequestDTO)
    {
        return await _ecoPointTransactions.AwardEcoPointsUserAsync(ecoPointRequestDTO);
    }

    public async Task<List<EcoPointTransactionDTO>> GetEcoActionHistory(Guid communityId, Guid userId)
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

    public async Task<CommunityEcoActionSummaryDTO> GetCommunityEcoActionSummary(Guid communityId, Guid requestingUserId)
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
}
