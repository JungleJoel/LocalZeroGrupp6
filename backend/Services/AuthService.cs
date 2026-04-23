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
        private readonly ICommunityService _communityService;
        private readonly IUserService _userService;

        public AuthService(ApplicationDbContext database, ICommunityService communityService, IUserService userService)
        {
            _database = database;
            _communityService = communityService;
            _userService = userService;
        }

        public async Task<UserDTO> AuthenticateAsync(LoginRequestDTO request)
        {
            var userEntity = await _database.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (userEntity == null)
            {
                throw new UnauthorizedException("Invalid email or password.");
            }

            if (BCrypt.Net.BCrypt.Verify(request.Password, userEntity.PasswordHash) == false)
            {
                throw new UnauthorizedException("Invalid email or password.");
            }

            var user = await _userService.GetAsync(userEntity.Id);
            return user.Adapt<UserDTO>();
        }

        public async Task<UserDTO> RegisterAsync(RegisterRequestDTO request)
        {
            var exists = await _database.Users.AnyAsync(u => u.Email == request.Email);

            if (exists)
                throw new ConflictException("A user with that email already exists.");

            var userEntity = new User
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow
            };

            _database.Users.Add(userEntity);
            await _database.SaveChangesAsync();

            var user = await _userService.GetAsync(userEntity.Id);

            return user.Adapt<UserDTO>();
        }

        public async Task<bool> IsManagerAtLoginAsync(Guid userId)
        {
            try
            {
                var userCommunity = await _communityService.GetMyCommunityAsync(userId);
                return userCommunity.IsCommunityManager;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
