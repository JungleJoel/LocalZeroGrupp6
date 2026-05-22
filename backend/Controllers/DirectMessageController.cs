using System.Security.Claims;
using backend.Interfaces;
using backend.Models.DTOs;
using backend.Models.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class DirectMessageController : BaseController
{
    private readonly IDirectMessageService _directMessageService;

    public DirectMessageController(IDirectMessageService directMessageService)
    {
        _directMessageService = directMessageService;
    }

    [HttpGet("conversations")]
    public async Task<ActionResult<List<ConversationSummaryDTO>>> GetConversations()
    {
        var conversations = await _directMessageService.GetConversationsAsync(GetUserId());
        return Ok(conversations);
    }

    [HttpGet("conversation/{otherUserId}")]
    public async Task<ActionResult<List<DirectMessageDTO>>> GetConversation(Guid otherUserId)
    {
        var messages = await _directMessageService.GetConversationAsync(GetUserId(), otherUserId);
        return Ok(messages);
    }

    [HttpPost]
    public async Task<ActionResult<DirectMessageDTO>> Send(SendMessageRequestDTO request)
    {
        var message = await _directMessageService.SendMessageAsync(GetUserId(), request);
        return Ok(message);
    }
}
