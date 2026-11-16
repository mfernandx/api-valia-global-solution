
using System.ComponentModel.DataAnnotations;

namespace ValiaApiWeb.DTOs.Requests
{
    public class UpdateCommunityCommentRequest
    {
        [Required(ErrorMessage = "O comentário é obrigatório.")]
        [MaxLength(2000)]
        public string Content { get; set; } = string.Empty; // Conteúdo atualizado
    }
}

