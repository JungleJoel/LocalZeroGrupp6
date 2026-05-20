namespace backend.Models.DTOs;

public record InitiativeCommentDTO(
    Guid Id,
    Guid InitiativeId,
    Guid UserId,
    string AuthorName,
    string Body,
    DateTime CreatedAt
);
