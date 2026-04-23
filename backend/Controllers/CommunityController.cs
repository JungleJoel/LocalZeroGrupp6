using System.Security.Claims;
using backend.Interfaces;
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
public class CommunityController(ICommunityService communityService) : ControllerBase
{
    [HttpGet("getCommunities")]
    public async Task<ActionResult<List<CommunityDTO>>> GetCommunities()
    {
        var communities = await communityService.GetCommunitiesAsync();
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
    public async Task<ActionResult<CommunityJoinRequestDTO>> JoinCommunities(Guid communityId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return await communityService.SubmitJoinRequestAsync(userId, communityId);
    }

    [Authorize(Roles = "CommunityManager")]
    [HttpGet("{communityId}/get-requests")]
    public async Task<ActionResult<List<CommunityJoinRequestDTO>>> GetRequests(Guid  communityId)
    {
        var managerUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return await communityService.GetRequestsAsync(managerUserId, communityId);
    }
    
    [Authorize(Roles = "CommunityManager")]
    [HttpPost("{communityId}/approve-request/{requestId}")]
    public async Task<ActionResult<CommunityJoinRequestDTO>> ApproveRequest(Guid requestId, Guid communityId)
    {
        var managerUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return await communityService.ApproveRequestAsync(requestId, managerUserId, communityId);
    }

    [Authorize(Roles = "CommunityManager")]
    [HttpPost("{communityId}/decline-request/{requestId}")]
    public async Task<ActionResult<CommunityJoinRequestDTO>> DeclineRequest(Guid requestId, Guid communityId)
    {
        var managerUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return  await communityService.DeclineRequestAsync(requestId, managerUserId, communityId);
    }

    [HttpPost("{communityId}/leave")]
    public async Task<ActionResult> LeaveCommunities(Guid communityId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await communityService.LeaveCommunityAsync(userId, communityId);
        return Ok();
    }

    /*[HttpGet("{communityId}/members")]
    public async Task<ActionResult<List<UserDTO>>> GetCommunityMembers(Guid communityId)
    {
        throw new NotImplementedException();
    }

    [HttpGet("{communityId}/members/{userId}")]
    public async Task<ActionResult<UserDTO>> GetCommunityMember(Guid communityId, Guid userId)
    {
        throw new NotImplementedException();
    }

    [Authorize(Roles = "CommunityManager")]
    [HttpPost("{communityId}/members/{userId}/remove")]
    public async Task<ActionResult> RemoveCommunityMember(Guid communityId, Guid userId)
    {
        throw new NotImplementedException();
    }*/
}