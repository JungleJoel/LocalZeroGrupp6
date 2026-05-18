using System.Security.Claims;
using backend.Exceptions;
using backend.Interfaces;
using backend.Models;
using backend.Models.DTOs.Requests;
using backend.Services.EcoPoint;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class EcoActionsController : ControllerBase
{
    private readonly EcoPointCommandInvoker _invoker;
    private readonly IEcoPointService _ecoPointService;

    public EcoActionsController(EcoPointCommandInvoker ecoPointCommandInvoker, IEcoPointService ecoPointService)
    {
        _ecoPointService = ecoPointService;
        _invoker = ecoPointCommandInvoker;
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

        EcoPointRequestDTO request = new EcoPointRequestDTO(
            CommunityId: communityId,
            UserId: userId,
            InitiativeId: null,
            Amount: ecoPointPayloadDto.Amount,
            Reason: ecoPointPayloadDto.Reason
        );

        return await _invoker.InvokeAsync(new AwardEcoPointsCommand(request));
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
