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
    public Task GetCommunityLeaderboard(int communityId)
    {
        throw new NotImplementedException();
    }
    
    [HttpGet("community/{communityId}/members/{userId}/balance")]
    public Task GetMemberBalance(int communityId, int userId)
    {
        throw new NotImplementedException();
    }
    
    [HttpGet("community/{communityId}/members/{userId}/history")]
    public Task GetMemberHistory(int communityId, int userId)
    {
        throw new NotImplementedException();
    }
    
    [Authorize(Roles = "CommunityManager")]
    [HttpPost("community/{communityId}/members/{userId}/award")]
    public Task AwardPointsMember(int communityId, int userId)
    {
        throw new NotImplementedException();
    }
    
    [HttpPost("community/{communityId}/members/{userId}/deduct")]
    public Task DeductPointsMember(int communityId, int userId)
    {
        throw new NotImplementedException();
    }
    
}