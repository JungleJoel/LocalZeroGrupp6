using backend.Models.DTOs;
using backend.Models.DTOs.Requests;

namespace backend.Interfaces;

public interface ICommentService
{
    Task<List<InitiativeCommentDTO>> GetCommentsAsync(Guid initiativeId, Guid userId);
    Task<InitiativeCommentDTO> CreateCommentAsync(Guid initiativeId, Guid userId, CreateCommentRequestDTO request);
    Task<InitiativeCommentDTO> LikeCommentAsync(Guid initiativeId, Guid commentId, Guid userId);
    Task UnlikeCommentAsync(Guid initiativeId, Guid commentId, Guid userId);
}
