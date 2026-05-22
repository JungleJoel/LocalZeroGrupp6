using backend.Interfaces;
using backend.Models.DTOs;
using backend.Models.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class InitiativeController : BaseController
{
    private readonly IInitiativeService _initiativeService;

    public InitiativeController(IInitiativeService initiativeService)
    {
        _initiativeService = initiativeService;
    }

    [HttpGet]
    public async Task<ActionResult<List<InitiativeDTO>>> GetAll()
    {
        var initiatives = await _initiativeService.GetInitiativesAsync();
        return Ok(initiatives);
    }

    [HttpGet("community/{communityId}")]
    public async Task<ActionResult<List<InitiativeDTO>>> GetByCommunity(Guid communityId)
    {
        var initiatives = await _initiativeService.GetByCommunityIdAsync(communityId, GetUserId());
        return Ok(initiatives);
    }
    

    [HttpGet("{id}")]
    public async Task<ActionResult<InitiativeDTO>> GetById(Guid id)
    {
        var initiative = await _initiativeService.GetInitiativeAsync(id, GetUserId());
        return Ok(initiative);
    }

    [HttpPost("create")]
    public async Task<ActionResult<InitiativeDTO>> Create(CreateInitiativeRequestDTO request)
    {
        var userId = GetUserId();
        var result = await _initiativeService.CreateInitiativeAsync(userId, request);
        
        
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpDelete("{id}/remove")]
    public async Task<ActionResult> Remove(Guid id)
    {
        var userId = GetUserId();
        await _initiativeService.RemoveInitiativeAsync(id, userId);
        return NoContent();
    }

    [HttpGet("{id}/participants")]
    public async Task<ActionResult<List<UserDTO>>> GetParticipants(Guid id)
    {
        var userId = GetUserId();
        var participants = await _initiativeService.GetParticipantsAsync(id, userId);
        return Ok(participants);
    }

    [HttpPost("{id}/end")]
    public async Task<ActionResult> End(Guid id)
    {
        var userId = GetUserId();
        await _initiativeService.EndInitiativeAsync(id, userId);
        return Ok();
    }

    [HttpPost("{initiativeId}/join")]
    public async Task<InitiativeDTO> JoinInitiative(Guid initiativeId)
    {
        return await _initiativeService.JoinInitiativeAsync(initiativeId, GetUserId());
    }

    [HttpDelete("{initiativeId}/leave")]
    public async Task<ActionResult> LeaveInitiative(Guid initiativeId)
    {
        await _initiativeService.LeaveInitiativeAsync(initiativeId, GetUserId());
        return Ok();
    }
}