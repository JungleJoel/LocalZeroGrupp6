namespace backend.Models.DTOs;

public record CommunityEcoPointBalanceDTO
(
    Guid CommunityId,
    int EcoPointBalance
);