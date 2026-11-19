using System.ComponentModel.DataAnnotations;

namespace ValiaApiWeb.DTOs.Requests;

public sealed class UpdateUserEmailRequest
{
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    public string Email { get; set; } = string.Empty;
}

