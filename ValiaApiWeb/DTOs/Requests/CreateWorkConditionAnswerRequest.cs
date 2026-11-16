using System.ComponentModel.DataAnnotations;

namespace ValiaApiWeb.DTOs.Requests
{
    public class CreateWorkConditionAnswerRequest
    {
        [Required(ErrorMessage = "A pergunta é obrigatória.")]
        public Guid? WorkConditionQuestionId { get; set; } // Pergunta respondida

        [Required(ErrorMessage = "A nota é obrigatória.")]
        [Range(1, 5, ErrorMessage = "A nota deve estar entre 1 e 5.")]
        public int? Score { get; set; } // Nota informada pelo trabalhador

        [MaxLength(1000)]
        public string? Observation { get; set; } // Observação opcional
    }
}

