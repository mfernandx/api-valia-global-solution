using System.ComponentModel.DataAnnotations;

namespace ValiaApiWeb.DTOs.Requests
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "O nome completo é obrigatório.")]
        [MaxLength(150)]
        public string FullName { get; set; } = string.Empty; // Nome completo do usuário

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty; // E-mail de cadastro

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [MinLength(8, ErrorMessage = "A senha deve possuir pelo menos 8 caracteres.")]
        public string Password { get; set; } = string.Empty; // Senha informada no cadastro

        [MaxLength(20)]
        public string? PhoneNumber { get; set; } // Telefone opcional
    }
}
