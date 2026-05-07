using backend.Data;
using backend.Exceptions;
using backend.Interfaces;
using backend.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _database;

        public AccountService(ApplicationDbContext database)
        {
            _database = database;
        }

        public async Task<AccountProfileDto> GetProfileAsync(Guid userId)
        {
            var user = await _database.Users.FindAsync(userId)
                ?? throw new NotFoundException("User not found.");

            return new AccountProfileDto(
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email,
                user.AvatarImageUrl,
                user.CreatedAt
            );
        }

        public async Task UpdateNameAsync(Guid userId, UpdateNameDto dto)
        {
            var user = await _database.Users.FindAsync(userId)
                ?? throw new NotFoundException("User not found.");

            user.FirstName = dto.FirstName.Trim();
            user.LastName = dto.LastName.Trim();

            await _database.SaveChangesAsync();
        }

        public async Task UpdateEmailAsync(Guid userId, UpdateEmailDto dto)
        {
            var user = await _database.Users.FindAsync(userId)
                ?? throw new NotFoundException("User not found.");

            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
                throw new UnauthorizedException("Current password is incorrect.");

            var emailTaken = await _database.Users.AnyAsync(u => u.Email == dto.NewEmail && u.Id != userId);
            if (emailTaken)
                throw new ConflictException("Email is already in use.");

            user.Email = dto.NewEmail.Trim().ToLower();

            await _database.SaveChangesAsync();
        }

        public async Task UpdatePasswordAsync(Guid userId, UpdatePasswordDto dto)
        {
            if (dto.NewPassword != dto.ConfirmNewPassword)
                throw new ConflictException("Passwords do not match.");

            if (dto.NewPassword.Length < 8)
                throw new ConflictException("Password must be at least 8 characters.");

            var user = await _database.Users.FindAsync(userId)
                ?? throw new NotFoundException("User not found.");

            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
                throw new UnauthorizedException("Current password is incorrect.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            await _database.SaveChangesAsync();
        }

        public async Task UpdateAvatarAsync(Guid userId, UpdateAvatarDto dto)
        {
            if (!Uri.TryCreate(dto.AvatarImageUrl, UriKind.Absolute, out _))
                throw new ConflictException("Invalid URL format.");

            var user = await _database.Users.FindAsync(userId)
                ?? throw new NotFoundException("User not found.");

            user.AvatarImageUrl = dto.AvatarImageUrl;

            await _database.SaveChangesAsync();
        }
    }
}