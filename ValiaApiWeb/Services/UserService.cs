
using Microsoft.EntityFrameworkCore;
using ValiaApiWeb.Data;
using ValiaApiWeb.Domain.Exceptions;
using ValiaApiWeb.Domain.Models;
using ValiaApiWeb.Interfaces;

namespace ValiaApiWeb.Services
{
    public class UserService : IUserService
    {
        private readonly ValiaDbContext _context;

        public UserService(ValiaDbContext context)
        {
            _context = context;
        }

        // Consulta um usuário pelo identificador
        public async Task<User?> GetByIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("Identificador do usuário inválido.");

            return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == userId);
        }

        // Verifica se um e-mail já está cadastrado
        public async Task<bool> EmailExistsAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("O e-mail é obrigatório.");

            string normalizedEmail = email.Trim().ToLowerInvariant();

            return await _context.Users.AnyAsync(u => u.Email == normalizedEmail);
        }

        // Atualiza somente o e-mail do usuário
        public async Task<User> UpdateEmailAsync(Guid userId, string email)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("Identificador do usuário inválido.");

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("O e-mail é obrigatório.");

            string normalizedEmail = email.Trim().ToLowerInvariant();

            User user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId) ?? throw new KeyNotFoundException("Usuário não encontrado.");

            bool emailInUse = await _context.Users.AnyAsync(u => u.Email == normalizedEmail && u.UserId != userId);

            if (emailInUse)
                throw new BusinessConflictException("Este e-mail já está cadastrado.");

            user.UpdateEmail(normalizedEmail);

            await _context.SaveChangesAsync();

            return user;
        }

        // Atualiza ou remove o telefone do usuário
        public async Task<User> UpdatePhoneNumberAsync(Guid userId, string? phoneNumber)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("Identificador do usuário inválido.");

            User user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId) ?? throw new KeyNotFoundException("Usuário não encontrado.");

            user.UpdatePhoneNumber(phoneNumber);

            await _context.SaveChangesAsync();

            return user;
        }

        // Desativa a conta sem excluir seus registros
        public async Task DeactivateAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("Identificador do usuário inválido.");

            User user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId) ?? throw new KeyNotFoundException("Usuário não encontrado.");

            user.Deactivate();

            await _context.SaveChangesAsync();
        }
    }
}

