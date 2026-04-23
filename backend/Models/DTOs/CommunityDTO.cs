namespace backend.Models.DTOs;

public record CommunityDTO (
    Guid Id,
    string Name,
    int EcoPoints,
    int ResidentsCount,
    double? Latitude,
    double? Longitude
);