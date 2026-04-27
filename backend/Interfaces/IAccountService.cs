using backend.Models.DTOs;

namespace backend.Interfaces
{
    public interface IAccountService
    {
        Task<AccountProfileDto> GetProfileAsync(Guid userId);
        Task UpdateNameAsync(Guid userId, UpdateNameDto dto);
        Task UpdateEmailAsync(Guid userId, UpdateEmailDto dto);
        Task UpdatePasswordAsync(Guid userId, UpdatePasswordDto dto);
        Task UpdateAvatarAsync(Guid userId, UpdateAvatarDto dto);
    }
}