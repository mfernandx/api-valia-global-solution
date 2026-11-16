
namespace ValiaApiWeb.DTOs.Responses
{
    public class UserResponse
    {
        public Guid UserId { get; set; } // Identificador do usuário
        public string FullName { get; set; } = string.Empty; // Nome completo
        public string Email { get; set; } = string.Empty; // E-mail
        public string? PhoneNumber { get; set; } // Telefone opcional
        public bool IsActive { get; set; } // Status da conta
        public DateTimeOffset CreatedAt { get; set; } // Data de criação
    }
}

