using ValiaApiWeb.Domain.Enums;

namespace ValiaApiWeb.DTOs.Responses
{
    public class EvidenceResponse
    {
        public Guid EvidenceId { get; set; } // Identificador do documento
        public Guid UserId { get; set; } // Proprietário
        public Guid? EmploymentId { get; set; } // Vínculo associado
        public string Title { get; set; } = string.Empty; // Título
        public EnumEvidenceCategory Category { get; set; } // Categoria
        public string FileName { get; set; } = string.Empty; // Nome original
        public string ContentType { get; set; } = string.Empty; // Tipo MIME
        public long FileSize { get; set; } // Tamanho em bytes
        public DateTimeOffset CreatedAt { get; set; } // Data de cadastro
    }
}
