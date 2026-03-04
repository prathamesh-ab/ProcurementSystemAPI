using ProcurementSystem.API.DTOs;

namespace ProcurementSystem.API.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> LoginAsync(LoginDTO dto);
        Task<AuthResponseDTO> RegisterAsync(RegisterDTO dto);
    }
}
