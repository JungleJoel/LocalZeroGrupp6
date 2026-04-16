namespace backend.Models.DTOs;

public record CommunityDTO (
    Guid Id,
    string Name,
    double? Latitude,
    double? Longitude
);
    