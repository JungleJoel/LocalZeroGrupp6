using System.Security.Claims;
using backend.Interfaces;
using backend.Models;
using backend.Models.DTOs;
using backend.Models.DTOs.Requests;
using backend.Services.EcoPoint;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class EcoPointController : ControllerBase
{
    private readonly EcoPointCommandInvoker _invoker;
    private readonly IEcoPointService _ecoPointService;

    public EcoPointController(IEcoPointService ecoPointService, EcoPointCommandInvoker ecoPointCommandInvoker)
    {
        _ecoPointService = ecoPointService;
        _invoker = ecoPointCommandInvoker;
    }
    
    [Authorize(Roles = "CommunityManager")]
    [HttpPost("community/{communityId}/user/{userId}/award")]
    public async Task<EcoPointTransactionDTO> AwardEcoPointsUser(Guid communityId, Guid userId, [FromBody] EcoPointPayloadDTO ecoPointPayloadDto)
    {
        return await _invoker.InvokeAsync(new AwardEcoPointsCommand(
            new EcoPointRequestDTO(
                communityId,
                userId,
                null,
                ecoPointPayloadDto.Amount,
                ecoPointPayloadDto.Reason)));
    }
    
    [Authorize(Roles = "CommunityManager")]
    [HttpPost("community/{communityId}/user/{userId}/deduct")]
    public async Task<EcoPointTransactionDTO> DeductEcoPointsUser(Guid communityId, Guid userId, [FromBody] EcoPointPayloadDTO ecoPointPayloadDto)
    {
        return await _invoker.InvokeAsync(new DeductEcoPointsCommand(
            new EcoPointRequestDTO(
                communityId,
                userId,
                null,
                ecoPointPayloadDto.Amount,
                ecoPointPayloadDto.Reason)));
    }
    
    [HttpGet("community/{communityId}/user/{userId}/balance")]
    public async Task<EcoPointBalanceDTO> GetUserEcoPointBalance(Guid communityId, Guid userId)
    {
        return await _ecoPointService.GetUserEcoPointBalanceAsync(communityId, userId);
    }
    
    [HttpGet("community/{communityId}/user/{userId}/history")]
    public async Task<List<EcoPointTransactionDTO>> GetUserEcoPointHistory(Guid communityId, Guid userId)
    {
        return await _ecoPointService.GetUserEcoPointHistoryAsync(communityId, userId);
    }

    [HttpGet("community/{communityId}/balance")]
    public async Task<EcoPointBalanceDTO> GetCommunityEcoPointBalance(Guid communityId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        return await _ecoPointService.GetCommunityEcoPointBalanceAsync(communityId, userId);
    }

    [HttpGet("community/{communityId}/history")]
    public async Task<List<EcoPointTransactionDTO>> GetCommunityEcoPointHistory(Guid communityId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        return await _ecoPointService.GetCommunityEcoPointHistoryAsync(communityId, userId);
    }
    
    [HttpGet("{communityId}/leaderboard")]
    public Task GetCommunityLeaderboard(Guid communityId)
    {
        throw new NotImplementedException();
    }
    
}