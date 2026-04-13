using backend.Models.DTOs;
using backend.Models.DTOs.Requests;

namespace backend.Interfaces
{
    public interface IAuthService
    {
        Task<UserDTO> AuthenticateAsync(LoginRequestDTO request);
        Task<UserDTO> RegisterAsync(RegisterRequestDTO request);
    }
}