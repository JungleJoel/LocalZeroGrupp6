using backend.Models.DTOs;
using backend.Models.DTOs.Requests;

namespace backend.Interfaces;

public interface IDirectMessageService
{
    Task<List<ConversationSummaryDTO>> GetConversationsAsync(Guid userId);
    Task<List<DirectMessageDTO>> GetConversationAsync(Guid userId, Guid otherUserId);
    Task<DirectMessageDTO> SendMessageAsync(Guid senderId, SendMessageRequestDTO request);
}
