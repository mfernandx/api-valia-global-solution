using ValiaApiWeb.Domain.Enums;

namespace ValiaApiWeb.Domain.Models
{
    public class CommunityPost
    {
        public Guid CommunityPostId { get; private set; } // Identificador único da publicação
        public Guid UserId { get; private set; } // Identificador do autor
        public EnumCommunityPostCategory Category { get; private set; } // Categoria da publicação
        public string Title { get; private set; } = string.Empty; // Título da publicação
        public string Content { get; private set; } = string.Empty; // Conteúdo textual da publicação
        public bool IsAnonymous { get; private set; } // Indica se o autor será ocultado publicamente
        public bool IsActive { get; private set; } // Indica se a publicação está disponível
        public DateTimeOffset CreatedAt { get; private set; } // Data de criação
        public DateTimeOffset? UpdatedAt { get; private set; } // Data da última atualização
        public int Version { get; private set; } // Versão para controle de concorrência
        public User User { get; private set; } = null!; // Usuário autor da publicação
        public ICollection<CommunityComment> Comments { get; private set; } = new List<CommunityComment>(); // Comentários associados à publicação

        protected CommunityPost() { }

        public CommunityPost(Guid userId, EnumCommunityPostCategory category, string title, string content, bool isAnonymous)
        {
            Validate(category, title, content);

            if (userId == Guid.Empty)
                throw new ArgumentException("Usuário inválido.");

            CommunityPostId = Guid.NewGuid();
            UserId = userId;
            Category = category;
            Title = title.Trim();
            Content = content.Trim();
            IsAnonymous = isAnonymous;
            IsActive = true;
            CreatedAt = DateTimeOffset.UtcNow;
            Version = 1;
        }


        public void Update(EnumCommunityPostCategory category, string title, string content, bool isAnonymous)
        {
            if (!Enum.IsDefined(category))
                throw new ArgumentException("Categoria da publicação inválida.");

            if (string.IsNullOrWhiteSpace(title) || title.Length > 150)
                throw new ArgumentException("O título deve conter entre 1 e 150 caracteres.");

            if (string.IsNullOrWhiteSpace(content) || content.Length > 5000)
                throw new ArgumentException("O conteúdo deve conter entre 1 e 5000 caracteres.");

            if (!IsActive)
                throw new InvalidOperationException("Não é possível editar uma publicação desativada.");

            Category = category;
            Title = title.Trim();
            Content = content.Trim();
            IsAnonymous = isAnonymous;
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

        private static void Validate(EnumCommunityPostCategory category, string title, string content)
        {
            if (!Enum.IsDefined(category))
                throw new ArgumentException("Categoria inválida.");

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("O título é obrigatório.");

            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("O conteúdo é obrigatório.");
        }
    }
}
