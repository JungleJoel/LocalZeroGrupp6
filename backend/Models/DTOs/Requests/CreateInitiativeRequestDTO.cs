namespace backend.Models.DTOs.Requests;

public record CreateInitiativeRequestDTO(
    Guid CommunityId,
    string Name,
    string Description,
    Guid? CategoryId,
    Guid? PresetId,
    bool IsPublic,
    double Latitude,
    double Longitude,
    DateTime StartsAt,
    DateTime? EstimatedEndsAt
);