
using ValiaApiWeb.DTOs.Requests;
using ValiaApiWeb.DTOs.Responses;

namespace ValiaApiWeb.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
    }
}

