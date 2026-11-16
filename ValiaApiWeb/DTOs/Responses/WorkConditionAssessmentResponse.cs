
namespace ValiaApiWeb.DTOs.Responses
{
    public class WorkConditionAssessmentResponse
    {
        public Guid WorkConditionAssessmentId { get; set; } // Identificador da avaliação
        public Guid EmploymentId { get; set; } // Vínculo avaliado
        public int QuestionnaireVersion { get; set; } // Versão utilizada
        public DateOnly AssessmentDate { get; set; } // Data da avaliação
        public DateTimeOffset CreatedAt { get; set; } // Data de criação
        public decimal AverageScore { get; set; } // Média geral
        public List<WorkConditionCategoryScoreResponse> CategoryScores { get; set; } = new(); // Médias por categoria
    }
}

