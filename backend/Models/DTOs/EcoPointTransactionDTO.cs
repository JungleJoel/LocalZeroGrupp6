namespace backend.Models;

public record EcoPointTransactionDTO
(
    Guid Id,
    Guid CommunityId,
    Guid UserId,
    Guid? InitiativeId,
    int Amount,
    DateTime CreatedAt 
);
    
    