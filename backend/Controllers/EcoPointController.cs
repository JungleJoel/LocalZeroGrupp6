using backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class EcoPointController(IEcoPointService ecoPointService) : ControllerBase
{
    
    [HttpGet("{communityId}/leaderboard")]
    public Task GetCommunityLeaderboard(Guid communityId)
    {
        throw new NotImplementedException();
    }
    
    [HttpGet("community/{communityId}/members/{userId}/balance")]
    public Task GetMemberBalance(Guid communityId, Guid userId)
    {
        throw new NotImplementedException();
    }
    
    [HttpGet("community/{communityId}/members/{userId}/history")]
    public Task GetMemberHistory(Guid communityId, Guid userId)
    {
        throw new NotImplementedException();
    }
    
    [Authorize(Roles = "CommunityManager")]
    [HttpPost("community/{communityId}/members/{userId}/award")]
    public Task AwardPointsMember(Guid communityId, Guid userId)
    {
        throw new NotImplementedException();
    }
    
    [Authorize(Roles = "CommunityManager")]
    [HttpPost("community/{communityId}/members/{userId}/deduct")]
    public Task DeductPointsMember(Guid communityId, Guid userId)
    {
        throw new NotImplementedException();
    }
    
}