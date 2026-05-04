using backend.Models.DTOs;

namespace backend.Interfaces
{
    public interface IUserService
    {
        Task<UserDTO> GetAsync(Guid id);
    }
}