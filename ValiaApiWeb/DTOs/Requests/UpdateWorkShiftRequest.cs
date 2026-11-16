
using System.ComponentModel.DataAnnotations;

namespace ValiaApiWeb.DTOs.Requests
{
    public class UpdateWorkShiftRequest
    {
        [Required(ErrorMessage = "O horário de entrada é obrigatório.")]
        public DateTimeOffset? StartTime { get; set; } // Novo horário de entrada

        [Required(ErrorMessage = "O horário de saída é obrigatório.")]
        public DateTimeOffset? EndTime { get; set; } // Novo horário de saída

        public DateTimeOffset? BreakStart { get; set; } // Novo início do intervalo

        public DateTimeOffset? BreakEnd { get; set; } // Novo fim do intervalo

        [MaxLength(1000)]
        public string? Notes { get; set; } // Observações atualizadas
    }
}

