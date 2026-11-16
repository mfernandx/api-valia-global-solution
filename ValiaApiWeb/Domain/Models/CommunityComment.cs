
using ValiaApiWeb.Domain.Models;

namespace ValiaApiWeb.Domain.Models
{
    public class CommunityComment
    {
        public Guid CommunityCommentId { get; private set; } // Identificador único do comentário
        public Guid CommunityPostId { get; private set; } // Identificador da publicação comentada
        public Guid UserId { get; private set; } // Identificador do autor do comentário
        public string Content { get; private set; } = string.Empty; // Conteúdo textual do comentário
        public bool IsAnonymous { get; private set; } // Indica se a identidade do autor será ocultada
        public bool IsActive { get; private set; } // Indica se o comentário está disponível
        public DateTimeOffset CreatedAt { get; private set; } // Data de criação do comentário
        public DateTimeOffset? UpdatedAt { get; private set; } // Data da última atualização
        public int Version { get; private set; } // Versão para controle de concorrência
        public CommunityPost CommunityPost { get; private set; } = null!; // Publicação associada
        public User User { get; private set; } = null!; // Usuário autor do comentário

        protected CommunityComment() { }

        public CommunityComment(Guid communityPostId, Guid userId, string content, bool isAnonymous)
        {
            if (communityPostId == Guid.Empty)
                throw new ArgumentException("A publicação é obrigatória.");

            if (userId == Guid.Empty)
                throw new ArgumentException("O autor é obrigatório.");

            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("O conteúdo do comentário é obrigatório.");

            CommunityCommentId = Guid.NewGuid();
            CommunityPostId = communityPostId;
            UserId = userId;
            Content = content.Trim();
            IsAnonymous = isAnonymous;
            IsActive = true;
            CreatedAt = DateTimeOffset.UtcNow;
            Version = 1;
        }

        public void UpdateContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("O conteúdo do comentário é obrigatório.");

            Content = content.Trim();
            UpdatedAt = DateTimeOffset.UtcNow;
            Version++;
        }

        public void Deactivate()
        {
            if (!IsActive)
                return;

            IsActive = false;
            UpdatedAt = DateTimeOffset.UtcNow;
            Version++;
        }
    }
}

