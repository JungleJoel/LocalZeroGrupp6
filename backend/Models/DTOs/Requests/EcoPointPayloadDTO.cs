namespace backend.Models.DTOs.Requests;

public record EcoPointPayloadDTO
(
    int Amount,
    string Reason
);