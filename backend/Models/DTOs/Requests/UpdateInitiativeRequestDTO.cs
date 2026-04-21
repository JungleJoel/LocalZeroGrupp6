namespace backend.Models.DTOs.Requests;

public record UpdateInitiativeRequestDTO(
    string Name,
    string Description,
    Guid? CategoryId,
    Guid? PresetId,
    bool IsPublic,
    double Latitude,
    double Longitude,
    DateTime StartsAt,
    DateTime? EstimatedEndsAt,
    DateTime? EndedAt
);