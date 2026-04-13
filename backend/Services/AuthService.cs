using backend.Data;
using backend.Exceptions;
using backend.Interfaces;
using backend.Models.DTOs;
using backend.Models.DTOs.Requests;
using backend.Models.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _database;

        public AuthService(ApplicationDbContext database)
        {
            _database = database;
        }

        public async Task<UserDTO> AuthenticateAsync(LoginRequestDTO request)
        {
            var user = await _database.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                throw new UnauthorizedException("Invalid email or password.");
            }

            if (BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash) == false)
            {
                throw new UnauthorizedException("Invalid email or password.");
            }

            return user.Adapt<UserDTO>();
        }

        public async Task<UserDTO> RegisterAsync(RegisterRequestDTO request)
        {
            var exists = await _database.Users.AnyAsync(u => u.Email == request.Email);

            if (exists)
                throw new ConflictException("A user with that email already exists.");

            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow
            };

            _database.Users.Add(user);
            await _database.SaveChangesAsync();

            return user.Adapt<UserDTO>();
        }
    }
}
