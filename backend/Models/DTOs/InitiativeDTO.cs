namespace backend.Models.DTOs;

public record InitiativeDTO(
    Guid Id,
    Guid CommunityId,
    Guid? CreatedBy,
    string Name,
    string Description,
    Guid? CategoryId,
    Guid? PresetId,
    bool IsPublic,
    double Latitude,
    double Longitude,
    DateTime StartsAt,
    DateTime? EstimatedEndsAt,
    DateTime? EndedAt,
    DateTime CreatedAt,
    int EcoPointsPerParticipant
);