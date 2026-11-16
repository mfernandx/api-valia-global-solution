
using Microsoft.AspNetCore.Identity;
using ValiaApiWeb.Domain.Models;
using ValiaApiWeb.Interfaces;

namespace ValiaApiWeb.Security
{
    public class PasswordService : IPasswordService
    {
        private readonly PasswordHasher<User> _passwordHasher = new();

        // Gera o hash seguro da senha
        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("A senha é obrigatória.");

            return _passwordHasher.HashPassword(null!, password);
        }

        // Verifica se a senha corresponde ao hash armazenado
        public bool VerifyPassword(string passwordHash, string password)
        {
            if (string.IsNullOrWhiteSpace(passwordHash) || string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            var result = _passwordHasher.VerifyHashedPassword(null!, passwordHash, password);

            return result != PasswordVerificationResult.Failed;
        }
    }
}

