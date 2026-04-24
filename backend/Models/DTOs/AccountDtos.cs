namespace backend.Models.DTOs;

public record UpdateNameDto(string FirstName, string LastName);
public record UpdateEmailDto(string NewEmail, string CurrentPassword);
public record UpdatePasswordDto(string CurrentPassword, string NewPassword, string ConfirmNewPassword);
public record UpdateAvatarDto(string AvatarImageUrl);
public record AccountProfileDto(Guid Id, string FirstName, string LastName, string Email, string? AvatarImageUrl, DateTime CreatedAt);