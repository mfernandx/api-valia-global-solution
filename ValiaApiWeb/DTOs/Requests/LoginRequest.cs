using System.ComponentModel.DataAnnotations;

namespace ValiaApiWeb.DTOs.Requests
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
        public string Email { get; set; } = string.Empty; // E-mail utilizado no login

        [Required(ErrorMessage = "A senha é obrigatória.")]
        public string Password { get; set; } = string.Empty; // Senha informada pelo usuário
    }
}
