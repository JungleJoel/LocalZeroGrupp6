using System.Security.Claims;
using backend.Interfaces;
using backend.Models.DTOs;
using backend.Models.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[Authorize]
[ApiController]
[Route("Initiative/{initiativeId}/comments")]
public class InitiativeCommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public InitiativeCommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpGet]
    public async Task<ActionResult<List<InitiativeCommentDTO>>> GetComments(Guid initiativeId)
    {
        var comments = await _commentService.GetCommentsAsync(initiativeId, GetUserId());
        return Ok(comments);
    }

    [HttpPost]
    public async Task<ActionResult<InitiativeCommentDTO>> CreateComment(Guid initiativeId, CreateCommentRequestDTO request)
    {
        if (string.IsNullOrWhiteSpace(request.Body))
            return BadRequest("Comment cannot be empty.");

        var comment = await _commentService.CreateCommentAsync(initiativeId, GetUserId(), request);
        return Ok(comment);
    }

    [HttpPost("{commentId}/like")]
    public async Task<ActionResult<InitiativeCommentDTO>> LikeComment(Guid initiativeId, Guid commentId)
    {
        var comment = await _commentService.LikeCommentAsync(initiativeId, commentId, GetUserId());
        return Ok(comment);
    }

    [HttpDelete("{commentId}/like")]
    public async Task<ActionResult> UnlikeComment(Guid initiativeId, Guid commentId)
    {
        await _commentService.UnlikeCommentAsync(initiativeId, commentId, GetUserId());
        return NoContent();
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim))
            throw new UnauthorizedAccessException("User ID not found in token");
        return Guid.Parse(userIdClaim);
    }
}
