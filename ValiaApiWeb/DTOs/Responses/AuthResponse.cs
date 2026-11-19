namespace ValiaApiWeb.DTOs.Responses
{
    public class AuthResponse
    {
        public Guid UserId { get; set; } // Identificador do usuário autenticado

        public string FullName { get; set; } = string.Empty; // Nome completo

        public string Email { get; set; } = string.Empty; // E-mail

        public string? PhoneNumber { get; set; } // Telefone opcional

        public string Token { get; set; } = string.Empty; // Token JWT

        public DateTimeOffset ExpiresAt { get; set; } // Data de expiração do token
    }
}

