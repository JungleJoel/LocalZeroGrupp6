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

    public async Task<List<InitiativeCommentDTO>> GetCommentsAsync(Guid initiativeId)
    {
        var initiativeExists = await _database.Initiatives.AnyAsync(i => i.Id == initiativeId);
        if (!initiativeExists)
            throw new NotFoundException("Initiative not found");

        return await _database.InitiativeComments
            .Include(c => c.User)
            .Where(c => c.InitiativeId == initiativeId)
            .OrderBy(c => c.CreatedAt)
            .Select(c => new InitiativeCommentDTO(
                c.Id,
                c.InitiativeId,
                c.UserId,
                $"{c.User.FirstName} {c.User.LastName}",
                c.Body,
                c.CreatedAt
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
            comment.CreatedAt
        );
    }
}
