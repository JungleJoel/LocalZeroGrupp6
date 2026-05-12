namespace backend.Models.DTOs.Responses;

public record MyJoinRequestDTO(
    Guid Id,
    Guid CommunityId,
    string CommunityName,
    DateTime CreatedAt);
