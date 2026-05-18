using System.Security.Claims;
using backend.Exceptions;
using backend.Interfaces;
using backend.Models;
using backend.Models.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class EcoActionsController : ControllerBase
{
    private readonly IEcoPointService _ecoPointService;

    public EcoActionsController(IEcoPointService ecoPointService)
    {
        _ecoPointService = ecoPointService;
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost("community/{communityId}/user/{userId}/eco-actions")]
    public async Task<EcoPointTransactionDTO> AddEcoAction(Guid communityId, Guid userId, [FromBody] EcoPointPayloadDTO ecoPointPayloadDto)
    {
        if (userId != GetUserId())
        {
            throw new ForbiddenException("You can only log eco-actions for yourself");
        }

        return await _ecoPointService.AwardEcoPointsUserAsync(new EcoPointRequestDTO(
            CommunityId: communityId,
            UserId: userId,
            InitiativeId: null,
            Amount: ecoPointPayloadDto.Amount,
            Reason: ecoPointPayloadDto.Reason
        ));
    }

    [HttpGet("community/{communityId}/user/{userId}/eco-actions")]
    public async Task<List<EcoPointTransactionDTO>> GetHistory(Guid communityId, Guid userId)
    {
        if (userId != GetUserId())
        {
            throw new ForbiddenException("You can only view your own eco-actions");
        }

        return await _ecoPointService.GetEcoActionHistoryAsync(communityId, userId);
    }

    [HttpGet("community/{communityId}/summary")]
    public async Task<CommunityEcoActionSummaryDTO> GetCommunitySummary(Guid communityId)
    {
        return await _ecoPointService.GetCommunityEcoActionSummaryAsync(communityId, GetUserId());
    }
}
