
using ValiaApiWeb.Domain.Enums;

namespace ValiaApiWeb.DTOs.Responses
{
    public class WorkOccurrenceResponse
    {
        public Guid WorkOccurrenceId { get; set; } // Identificador da ocorrência
        public Guid EmploymentId { get; set; } // Vínculo profissional associado
        public OccurrenceCategory Category { get; set; } // Categoria da ocorrência
        public string Description { get; set; } = string.Empty; // Relato registrado
        public DateTimeOffset OccurredAt { get; set; } // Data do acontecimento
        public DateTimeOffset CreatedAt { get; set; } // Data de criação do registro
        public DateTimeOffset? UpdatedAt { get; set; } // Última atualização
        public int Version { get; set; } // Versão do registro
    }
}

