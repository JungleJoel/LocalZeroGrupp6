namespace backend.Models.DTOs.Responses;

public record CommunityMemberDTO(
    Guid UserId,
    string FirstName,
    string LastName,
    string? AvatarImageUrl,
    bool IsManager,
    DateTime JoinedAt);
