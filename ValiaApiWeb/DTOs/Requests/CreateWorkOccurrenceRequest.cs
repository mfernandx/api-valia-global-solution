
using System.ComponentModel.DataAnnotations;
using ValiaApiWeb.Domain.Enums;

namespace ValiaApiWeb.DTOs.Requests
{
    public class CreateWorkOccurrenceRequest
    {
        [Required(ErrorMessage = "O vínculo profissional é obrigatório.")]
        public Guid? EmploymentId { get; set; } // Vínculo relacionado à ocorrência

        [Required(ErrorMessage = "A categoria é obrigatória.")]
        public OccurrenceCategory? Category { get; set; } // Tipo de ocorrência

        [Required(ErrorMessage = "A descrição é obrigatória.")]
        [MaxLength(3000)]
        public string Description { get; set; } = string.Empty; // Relato da situação

        [Required(ErrorMessage = "A data da ocorrência é obrigatória.")]
        public DateTimeOffset? OccurredAt { get; set; } // Data e horário do acontecimento
    }
}

