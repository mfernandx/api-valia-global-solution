
using ValiaApiWeb.Domain.Enums;

namespace ValiaApiWeb.DTOs.Responses
{
    public class EmploymentResponse
    {
        public Guid EmploymentId { get; set; } // Identificador do vínculo
        public Guid UserId { get; set; } // Trabalhador proprietário
        public string JobTitle { get; set; } = string.Empty; // Cargo ou função
        public EnumEmploymentType EmploymentType { get; set; } // Tipo de vínculo
        public DateOnly AdmissionDate { get; set; } // Data de admissão
        public DateOnly? TerminationDate { get; set; } // Data de encerramento
        public decimal? BaseSalary { get; set; } // Salário-base
        public decimal? WeeklyWorkload { get; set; } // Horas semanais
        public string? ProfessionalCategory { get; set; } // Categoria profissional
        public int Version { get; set; } // Versão do registro
        public DateTimeOffset CreatedAt { get; set; } // Data de criação
        public DateTimeOffset? UpdatedAt { get; set; } // Última atualização
    }
}

