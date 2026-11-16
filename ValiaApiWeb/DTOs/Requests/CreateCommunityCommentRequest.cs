
using System.ComponentModel.DataAnnotations;

namespace ValiaApiWeb.DTOs.Requests
{
    public class CreateCommunityCommentRequest
    {
        [Required(ErrorMessage = "A publicação é obrigatória.")]
        public Guid? CommunityPostId { get; set; } // Publicação que receberá o comentário

        [Required(ErrorMessage = "O comentário é obrigatório.")]
        [MaxLength(2000)]
        public string Content { get; set; } = string.Empty; // Conteúdo do comentário

        public bool IsAnonymous { get; set; } // Define se o comentário será anônimo
    }
}

