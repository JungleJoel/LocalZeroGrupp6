namespace backend.Models.DTOs.Requests;

public record InitiativeEcoPointRequestDTO
(
    Guid InitiativeId,
    List<UserDTO> Users,
    int EcoPointAmount
);