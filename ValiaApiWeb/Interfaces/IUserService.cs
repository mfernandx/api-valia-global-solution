using ValiaApiWeb.Domain.Models;

namespace ValiaApiWeb.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetByIdAsync(Guid userId);
        Task<User> UpdateEmailAsync(Guid userId, string email);
        Task<User> UpdatePhoneNumberAsync(Guid userId, string? phoneNumber);
        Task DeactivateAsync(Guid userId);
        Task<bool> EmailExistsAsync(string email);
    }
}

