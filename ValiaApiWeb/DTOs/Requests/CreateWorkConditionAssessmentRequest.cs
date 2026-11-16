
using System.ComponentModel.DataAnnotations;
using ValiaApiWeb.DTOs.Requests;

namespace ValiaApiWeb.DTOs.Requests
{
    public class CreateWorkConditionAssessmentRequest
    {
        [Required(ErrorMessage = "O vínculo profissional é obrigatório.")]
        public Guid? EmploymentId { get; set; } // Vínculo avaliado

        [Required(ErrorMessage = "A data da avaliação é obrigatória.")]
        public DateOnly? AssessmentDate { get; set; } // Data da avaliação

        [Required(ErrorMessage = "As respostas são obrigatórias.")]
        [MinLength(1, ErrorMessage = "Informe pelo menos uma resposta.")]
        public List<CreateWorkConditionAnswerRequest> Answers { get; set; } = new(); // Respostas
    }
}

