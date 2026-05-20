using backend.Models.DTOs;
using backend.Models.DTOs.Requests;

namespace backend.Interfaces;

public interface ICommentService
{
    Task<List<InitiativeCommentDTO>> GetCommentsAsync(Guid initiativeId);
    Task<InitiativeCommentDTO> CreateCommentAsync(Guid initiativeId, Guid userId, CreateCommentRequestDTO request);
}
