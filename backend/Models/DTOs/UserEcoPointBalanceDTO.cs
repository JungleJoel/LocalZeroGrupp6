namespace backend.Models.DTOs;

public record UserEcoPointBalanceDTO
(
    Guid UserId,
    int EcoPointBalance
);