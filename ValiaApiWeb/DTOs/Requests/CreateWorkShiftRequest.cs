
using System.ComponentModel.DataAnnotations;

namespace ValiaApiWeb.DTOs.Requests
{
    public class CreateWorkShiftRequest
    {
        [Required(ErrorMessage = "O vínculo profissional é obrigatório.")]
        public Guid? EmploymentId { get; set; } // Vínculo ao qual a jornada pertence

        [Required(ErrorMessage = "O horário de entrada é obrigatório.")]
        public DateTimeOffset? StartTime { get; set; } // Data e hora de entrada

        [Required(ErrorMessage = "O horário de saída é obrigatório.")]
        public DateTimeOffset? EndTime { get; set; } // Data e hora de saída

        public DateTimeOffset? BreakStart { get; set; } // Início do intervalo opcional

        public DateTimeOffset? BreakEnd { get; set; } // Fim do intervalo opcional

        [MaxLength(1000)]
        public string? Notes { get; set; } // Observações sobre a jornada
    }
}

