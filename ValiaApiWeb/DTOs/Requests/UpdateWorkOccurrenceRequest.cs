
using System.ComponentModel.DataAnnotations;
using ValiaApiWeb.Domain.Enums;

namespace ValiaApiWeb.DTOs.Requests
{
    public class UpdateWorkOccurrenceRequest
    {
        [Required(ErrorMessage = "A categoria é obrigatória.")]
        public OccurrenceCategory? Category { get; set; } // Categoria atualizada

        [Required(ErrorMessage = "A descrição é obrigatória.")]
        [MaxLength(3000)]
        public string Description { get; set; } = string.Empty; // Descrição atualizada

        [Required(ErrorMessage = "A data da ocorrência é obrigatória.")]
        public DateTimeOffset? OccurredAt { get; set; } // Data e horário atualizados
    }
}

