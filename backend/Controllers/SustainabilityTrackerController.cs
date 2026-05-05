using backend.Interfaces;
using backend.Models;
using backend.Models.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("[controller]")]
public class SustainabilityTrackerController
{
    
    private readonly ISustainabilityTrackerService _sustainabilityTrackerService;

    public SustainabilityTrackerController(ISustainabilityTrackerService sustainabilityTrackerService)
    {
        _sustainabilityTrackerService = sustainabilityTrackerService;
    }

    [Authorize]
    [HttpPost("community/{communityId}/user/{userId}/sustainability")]
    public async Task<EcoPointTransactionDTO> AddSustainableEcoPoints(Guid communityId, Guid userId, [FromBody] EcoPointPayloadDTO ecoPointPayloadDto)
    {
         return await _sustainabilityTrackerService.AwardSustainability(new EcoPointRequestDTO(
             CommunityId: communityId,
             UserId: userId,
             InitiativeId: null,
             Amount: ecoPointPayloadDto.Amount,
             Reason: ecoPointPayloadDto.Reason
             ));
    }
    
    [Authorize]
    [HttpGet("community/{communityId}/user/{userId}/sustainability")]
    public async Task<List<EcoPointTransactionDTO>> GetHistory(Guid userId)
    {
        return await _sustainabilityTrackerService.GetSustainabilityTrackerHistory(userId);
    }
    
}

