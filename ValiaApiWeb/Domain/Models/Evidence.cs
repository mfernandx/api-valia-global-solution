
using ValiaApiWeb.Domain.Enums;
using ValiaApiWeb.Domain.Models;

namespace ValiaApiWeb.Domain.Models
{
    public class Evidence
    {
        public Guid EvidenceId { get; private set; } // Identificador único do documento
        public Guid UserId { get; private set; } // Usuário proprietário do documento
        public Guid? EmploymentId { get; private set; } // Vínculo profissional opcional
        public string Title { get; private set; } = string.Empty; // Título do documento
        public EnumEvidenceCategory Category { get; private set; } // Categoria do documento
        public string FileName { get; private set; } = string.Empty; // Nome original do arquivo
        public string StorageKey { get; private set; } = string.Empty; // Identificador interno no armazenamento privado
        public string ContentType { get; private set; } = string.Empty; // Tipo MIME do arquivo
        public long FileSize { get; private set; } // Tamanho do arquivo em bytes
        public DateTimeOffset CreatedAt { get; private set; } // Data de cadastro
        public User User { get; private set; } = null!; // Usuário proprietário
        public Employment? Employment { get; private set; } // Vínculo associado, quando existir
        
        protected Evidence() { }

        public Evidence(Guid userId, Guid? employmentId, string title, EnumEvidenceCategory category, string fileName, string storageKey, string contentType, long fileSize)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("Usuário inválido.");

            if (employmentId == Guid.Empty)
                throw new ArgumentException("Vínculo profissional inválido.");

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("O título é obrigatório.");

            if (!Enum.IsDefined(category))
                throw new ArgumentException("Categoria inválida.");

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("O nome do arquivo é obrigatório.");

            if (string.IsNullOrWhiteSpace(storageKey))
                throw new ArgumentException("O identificador do arquivo é obrigatório.");

            if (string.IsNullOrWhiteSpace(contentType))
                throw new ArgumentException("O tipo do arquivo é obrigatório.");

            if (fileSize <= 0)
                throw new ArgumentException("O arquivo deve possuir conteúdo.");

            EvidenceId = Guid.NewGuid();
            UserId = userId;
            EmploymentId = employmentId;
            Title = title.Trim();
            Category = category;
            FileName = fileName.Trim();
            StorageKey = storageKey;
            ContentType = contentType;
            FileSize = fileSize;
            CreatedAt = DateTimeOffset.UtcNow;
        }
    }
}

