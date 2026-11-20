using System.ComponentModel.DataAnnotations;
using ValiaApiWeb.Domain.Enums;

namespace ValiaApiWeb.DTOs.Requests
{
    public class UpdateCommunityPostRequest
    {
        [Required(ErrorMessage = "A categoria é obrigatória.")]
        public EnumCommunityPostCategory? Category { get; set; } // Categoria atualizada

        [Required(ErrorMessage = "O título é obrigatório.")]
        [MaxLength(150)]
        public string Title { get; set; } = string.Empty; // Título atualizado

        [Required(ErrorMessage = "O conteúdo é obrigatório.")]
        [MaxLength(5000)]
        public string Content { get; set; } = string.Empty; // Conteúdo atualizado

        public bool IsAnonymous { get; set; } // Configuração de anonimato
    }
}

