
using ValiaApiWeb.Domain.Models;

namespace ValiaApiWeb.Domain.Models
{
    public class WorkConditionAnswer
    {
        public Guid WorkConditionAnswerId { get; private set; } // Identificador da resposta
        public Guid WorkConditionAssessmentId { get; private set; } // Avaliação à qual pertence
        public Guid WorkConditionQuestionId { get; private set; } // Pergunta respondida
        public int Score { get; private set; } // Resposta numérica de 1 a 5
        public string? Observation { get; private set; } // Observação opcional do trabalhador
        public WorkConditionAssessment Assessment { get; private set; } = null!; // Avaliação associada
        public WorkConditionQuestion Question { get; private set; } = null!; // Pergunta associada

        protected WorkConditionAnswer() { }

        public WorkConditionAnswer(Guid assessmentId, Guid questionId, int score, string? observation)
        {
            if (assessmentId == Guid.Empty || questionId == Guid.Empty)
                throw new ArgumentException("A avaliação e a pergunta são obrigatórias.");

            if (score < 1 || score > 5)
                throw new ArgumentException("A resposta deve estar entre 1 e 5.");

            WorkConditionAnswerId = Guid.NewGuid();
            WorkConditionAssessmentId = assessmentId;
            WorkConditionQuestionId = questionId;
            Score = score;
            Observation = observation?.Trim();
        }
    }
}

