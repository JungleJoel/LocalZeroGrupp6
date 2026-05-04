using backend.Data;
using backend.Exceptions;
using backend.Interfaces;
using backend.Models.DTOs;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _database;

        public UserService(ApplicationDbContext database)
        {
            _database = database;
        }

        public async Task<UserDTO> GetAsync(Guid id)
        {
            var user = await _database.Users
                .Include(u => u.CommunityResidents)
                    .ThenInclude(cr => cr.Community)
                .Include(u => u.EcoPointTransactions)
                .AsSplitQuery()
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                throw new NotFoundException("User not found by passed id.");
            }

            return user.Adapt<UserDTO>();
        }
    }
}
