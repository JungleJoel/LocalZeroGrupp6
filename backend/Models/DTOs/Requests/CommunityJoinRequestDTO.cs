namespace backend.Models.DTOs.Requests;

public record CommunityJoinRequestDTO 
(
    Guid Id,
    Guid UserId,
    Guid CommunityId,
    bool? IsAccepted,
    DateTime CreatedAt
);