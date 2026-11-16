
using ValiaApiWeb.Domain.Enums;

namespace ValiaApiWeb.DTOs.Responses
{
    public class CommunityPostResponse
    {
        public Guid CommunityPostId { get; set; } // Identificador da publicação
        public Guid? UserId { get; set; } // Identificador do autor, ocultado em publicações anônimas
        public string AuthorName { get; set; } = string.Empty; // Nome exibido na comunidade
        public EnumCommunityPostCategory Category { get; set; } // Categoria
        public string Title { get; set; } = string.Empty; // Título
        public string Content { get; set; } = string.Empty; // Conteúdo
        public bool IsAnonymous { get; set; } // Indica anonimato
        public DateTimeOffset CreatedAt { get; set; } // Data de criação
        public DateTimeOffset? UpdatedAt { get; set; } // Última atualização
        public int Version { get; set; } // Versão do registro
        public int CommentCount { get; set; } // Quantidade de comentários ativos
        public bool IsOwner { get; set; } // Indica se pertence ao usuário autenticado
    }
}

