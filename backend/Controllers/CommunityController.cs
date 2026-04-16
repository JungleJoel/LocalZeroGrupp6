using System.Security.Claims;
using backend.Interfaces;
using backend.Models.DTOs;
using backend.Models.DTOs.Requests;
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
    
    [HttpGet("get")]
    public async Task<ActionResult<List<CommunityDTO>>> GetCommunities()
    {
        var communities = await _communityService.GetCommunitiesAsync();
        return Ok(communities);
    }

    [HttpPost("{communityId}/join")]
    public async Task<CommunityJoinRequestDTO> JoinCommunities(Guid communityId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return await _communityService.SubmitJoinRequestAsync(userId, communityId);
    }
    
    [HttpGet("{communityId}/get-requests")]
    
    [HttpPost("{communityId}/approve-request/{requestId}")]
    public async Task ApproveRequest(Guid communityId, Guid requestId)
    {
        throw new NotImplementedException();
    }

    [HttpPost("{communityId}/leave")]
    public async Task LeaveCommunities(Guid communityId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _communityService.LeaveCommunityAsync(userId, communityId);
    }
}