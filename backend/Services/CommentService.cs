using backend.Data;
using backend.Exceptions;
using backend.Interfaces;
using backend.Models.DTOs;
using backend.Models.DTOs.Requests;
using backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class CommentService : ICommentService
{
    private readonly ApplicationDbContext _database;

    public CommentService(ApplicationDbContext database)
    {
        _database = database;
    }

    public async Task<List<InitiativeCommentDTO>> GetCommentsAsync(Guid initiativeId, Guid userId)
    {
        var initiativeExists = await _database.Initiatives.AnyAsync(i => i.Id == initiativeId);
        if (!initiativeExists)
            throw new NotFoundException("Initiative not found");

        return await _database.InitiativeComments
            .Include(c => c.User)
            .Include(c => c.InitiativeCommentLikes)
            .Where(c => c.InitiativeId == initiativeId)
            .OrderBy(c => c.CreatedAt)
            .Select(c => new InitiativeCommentDTO(
                c.Id,
                c.InitiativeId,
                c.UserId,
                $"{c.User.FirstName} {c.User.LastName}",
                c.Body,
                c.CreatedAt,
                c.InitiativeCommentLikes.Count,
                c.InitiativeCommentLikes.Any(l => l.UserId == userId)
            ))
            .ToListAsync();
    }

    public async Task<InitiativeCommentDTO> CreateCommentAsync(Guid initiativeId, Guid userId, CreateCommentRequestDTO request)
    {
        var body = request.Body.Trim();
        if (string.IsNullOrWhiteSpace(body))
            throw new ArgumentException("Comment cannot be empty.");

        var initiativeExists = await _database.Initiatives.AnyAsync(i => i.Id == initiativeId);
        if (!initiativeExists)
            throw new NotFoundException("Initiative not found");

        var comment = new InitiativeComment
        {
            Id = Guid.NewGuid(),
            InitiativeId = initiativeId,
            UserId = userId,
            Body = body,
            CreatedAt = DateTime.UtcNow
        };

        await _database.InitiativeComments.AddAsync(comment);
        await _database.SaveChangesAsync();

        var user = await _database.Users.FindAsync(userId);
        var authorName = user == null ? "Unknown user" : $"{user.FirstName} {user.LastName}";

        return new InitiativeCommentDTO(
            comment.Id,
            comment.InitiativeId,
            comment.UserId,
            authorName,
            comment.Body,
            comment.CreatedAt,
            0,
            false
        );
    }

    public async Task<InitiativeCommentDTO> LikeCommentAsync(Guid initiativeId, Guid commentId, Guid userId)
    {
        var comment = await GetCommentAsync(initiativeId, commentId);

        if (!comment.InitiativeCommentLikes.Any(l => l.UserId == userId))
        {
            var like = new InitiativeCommentLike
            {
                CommentId = commentId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _database.InitiativeCommentLikes.AddAsync(like);

            await _database.SaveChangesAsync();
        }

        return ToCommentDto(comment, userId);
    }

    public async Task UnlikeCommentAsync(Guid initiativeId, Guid commentId, Guid userId)
    {
        var like = await _database.InitiativeCommentLikes
            .Include(l => l.Comment)
            .FirstOrDefaultAsync(l =>
                l.CommentId == commentId &&
                l.UserId == userId &&
                l.Comment.InitiativeId == initiativeId);

        if (like == null)
            return;

        _database.InitiativeCommentLikes.Remove(like);
        await _database.SaveChangesAsync();
    }

    private async Task<InitiativeComment> GetCommentAsync(Guid initiativeId, Guid commentId)
    {
        var comment = await _database.InitiativeComments
            .Include(c => c.User)
            .Include(c => c.InitiativeCommentLikes)
            .FirstOrDefaultAsync(c => c.Id == commentId && c.InitiativeId == initiativeId);

        if (comment == null)
            throw new NotFoundException("Comment not found");

        return comment;
    }

    private static InitiativeCommentDTO ToCommentDto(InitiativeComment comment, Guid userId)
    {
        return new InitiativeCommentDTO(
            comment.Id,
            comment.InitiativeId,
            comment.UserId,
            $"{comment.User.FirstName} {comment.User.LastName}",
            comment.Body,
            comment.CreatedAt,
            comment.InitiativeCommentLikes.Count,
            comment.InitiativeCommentLikes.Any(l => l.UserId == userId)
        );
    }
}
