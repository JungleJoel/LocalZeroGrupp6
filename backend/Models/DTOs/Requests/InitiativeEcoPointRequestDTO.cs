namespace backend.Models.DTOs.Requests;

public record InitiativeEcoPointRequestDTO(
    Guid InitiativeId,
    Dictionary<Guid, Guid> Participants,
    int EcoPointAmount
);