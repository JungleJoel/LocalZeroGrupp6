using System.Security.Claims;
using backend.Interfaces;
using backend.Models;
using backend.Models.DTOs;
using backend.Models.DTOs.Requests;
using backend.Models.DTOs.Responses;
using backend.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class CommunityController : ControllerBase
{
    
    private readonly ICommunityService _communityService;

    public CommunityController(ICommunityService communityService)
    {
        _communityService = communityService;
    }
    
    [HttpGet("getCommunities")]
    public async Task<ActionResult<List<CommunityDTO>>> GetCommunities()
    {
        var communities = await _communityService.GetCommunitiesAsync();
        return Ok(communities);
    }
    
    [HttpGet("my-community")]
    public async Task<ActionResult<GetMyCommunityResponseDTO>> GetMyCommunity()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var community = await _communityService.GetMyCommunityAsync(userId);
        return Ok(community);
            
    }

    [HttpPost("{communityId}/join")]
    public async Task<CommunityJoinRequestDTO> JoinCommunities(Guid communityId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return await _communityService.SubmitJoinRequestAsync(userId, communityId);
    }

    [Authorize(Roles = "CommunityManager")]
    [HttpGet("{communityId}/get-requests")]
    public async Task<List<CommunityJoinRequestDTO>> GetRequests(Guid  communityId)
    {
        var managerUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return await _communityService.GetRequestsAsync(managerUserId, communityId);
    }
    
    [Authorize(Roles = "CommunityManager")]
    [HttpPost("{communityId}/approve-request/{requestId}")]
    public async Task<ActionResult<CommunityJoinRequestDTO>> ApproveRequest(Guid requestId, Guid communityId)
    {
        var managerUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return await _communityService.ApproveRequestAsync(requestId, managerUserId, communityId);
    }

    [Authorize(Roles = "CommunityManager")]
    [HttpPost("{communityId}/decline-request/{requestId}")]
    public async Task<ActionResult<CommunityJoinRequestDTO>> DeclineRequest(Guid requestId, Guid communityId)
    {
        var managerUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return  await _communityService.DeclineRequestAsync(requestId, managerUserId, communityId);
    }

    [HttpPost("{communityId}/leave")]
    public async Task LeaveCommunities(Guid communityId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _communityService.LeaveCommunityAsync(userId, communityId);
    }
}