namespace backend.Models.DTOs;

public record UserDTO
    (
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        string? AvatarImageUrl,
        int EcoPoints,
        bool? IsCommunityManager,
        CommunityDTO? Community,
        DateTime CreatedAt
    );