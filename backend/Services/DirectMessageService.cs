using backend.Data;
using backend.Exceptions;
using backend.Interfaces;
using backend.Models.DTOs;
using backend.Models.DTOs.Requests;
using backend.Models.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class DirectMessageService : IDirectMessageService
{
    private readonly ApplicationDbContext _database;

    public DirectMessageService(ApplicationDbContext context)
    {
        _database = context;
    }

    public async Task<List<ConversationSummaryDTO>> GetConversationsAsync(Guid userId)
    {
        var messages = await _database.DirectMessages
            .Include(m => m.Sender)
            .Include(m => m.Recipient)
            .Where(m => m.SenderId == userId || m.RecipientId == userId)
            .ToListAsync();

        return messages
            .GroupBy(m => m.SenderId == userId ? m.RecipientId : m.SenderId)
            .Select(g =>
            {
                var latest = g.OrderByDescending(m => m.CreatedAt).First();
                var otherUser = latest.SenderId == userId ? latest.Recipient : latest.Sender;
                return new ConversationSummaryDTO
                {
                    UserId = otherUser.Id,
                    FirstName = otherUser.FirstName,
                    LastName = otherUser.LastName,
                    AvatarImageUrl = otherUser.AvatarImageUrl,
                    LastMessage = latest.Body,
                    LastMessageAt = latest.CreatedAt
                };
            })
            .OrderByDescending(c => c.LastMessageAt)
            .ToList();
    }

    public async Task<List<DirectMessageDTO>> GetConversationAsync(Guid userId, Guid otherUserId)
    {
        return await _database.DirectMessages
            .Where(m =>
                (m.SenderId == userId && m.RecipientId == otherUserId) ||
                (m.SenderId == otherUserId && m.RecipientId == userId))
            .OrderBy(m => m.CreatedAt)
            .ProjectToType<DirectMessageDTO>()
            .ToListAsync();
    }

    public async Task<DirectMessageDTO> SendMessageAsync(Guid senderId, SendMessageRequestDTO request)
    {
        if (string.IsNullOrWhiteSpace(request.Body))
            throw new ArgumentException("Message body cannot be empty.");

        var message = new DirectMessage
        {
            Id = Guid.NewGuid(),
            SenderId = senderId,
            RecipientId = request.RecipientId,
            Body = request.Body.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        await _database.DirectMessages.AddAsync(message);
        await _database.SaveChangesAsync();

        return message.Adapt<DirectMessageDTO>();
    }
}
