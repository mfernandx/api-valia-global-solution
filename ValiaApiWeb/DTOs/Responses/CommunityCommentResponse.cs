namespace ValiaApiWeb.DTOs.Responses
{
    public class CommunityCommentResponse
    {
        public Guid CommunityCommentId { get; set; } // Identificador do comentário
        public Guid CommunityPostId { get; set; } // Publicação associada
        public Guid? UserId { get; set; } // Autor, ocultado quando anônimo
        public string AuthorName { get; set; } = string.Empty; // Nome exibido
        public string Content { get; set; } = string.Empty; // Conteúdo
        public bool IsAnonymous { get; set; } // Indica anonimato
        public DateTimeOffset CreatedAt { get; set; } // Data de criação
        public DateTimeOffset? UpdatedAt { get; set; } // Última atualização
        public int Version { get; set; } // Versão do registro
        public bool IsOwner { get; set; } // Permissão de edição/exclusão
    }
}

