namespace backend.Models.DTOs.Requests;

public record LoginRequestDTO
    (
    string Email,
    string Password
    );