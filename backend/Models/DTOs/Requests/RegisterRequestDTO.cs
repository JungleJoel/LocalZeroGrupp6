namespace backend.Models.DTOs.Requests;

public record RegisterRequestDTO
    (
    string FirstName,
    string LastName,
    string Email,
    string Password
    );