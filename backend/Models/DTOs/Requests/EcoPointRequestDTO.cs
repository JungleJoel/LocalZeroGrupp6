namespace backend.Models.DTOs.Requests;

public record EcoPointRequestDTO
(   
    Guid CommunityId,
    Guid UserId,
    Guid? InitiativeId,
    int Amount
);