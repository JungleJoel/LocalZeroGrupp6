namespace backend.Models.DTOs;

public record UserDTO
    (
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        string? AvatarImageUrl,
        CommunityDTO? Community,
        int EcoPoints,
        DateTime CreatedAt
    );