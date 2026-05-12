namespace backend.Models.DTOs.Responses;

public record CommunityJoinRequestWithUserDTO(
    Guid Id,
    Guid UserId,
    string UserFirstName,
    string UserLastName,
    Guid CommunityId,
    bool? IsAccepted,
    DateTime CreatedAt);
