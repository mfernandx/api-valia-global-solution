namespace ValiaApiWeb.Domain.Models
{
    public class WorkConditionAssessment
    {
        public Guid WorkConditionAssessmentId { get; private set; } // Identificador da avaliação
        public Guid EmploymentId { get; private set; } // Vínculo profissional avaliado
        public int QuestionnaireVersion { get; private set; } // Versão utilizada na avaliação
        public DateOnly AssessmentDate { get; private set; } // Data da avaliação
        public DateTimeOffset CreatedAt { get; private set; } // Data de criação do registro
        public Employment Employment { get; private set; } = null!; // Vínculo avaliado
        public ICollection<WorkConditionAnswer> Answers { get; private set; } = new List<WorkConditionAnswer>(); // Respostas da avaliação

        private WorkConditionAssessment() { }

        public WorkConditionAssessment(Guid employmentId, int questionnaireVersion, DateOnly assessmentDate)
        {
            if (employmentId == Guid.Empty)
                throw new ArgumentException("O vínculo profissional é obrigatório.");

            if (questionnaireVersion <= 0)
                throw new ArgumentException("A versão do questionário deve ser maior que zero.");

            WorkConditionAssessmentId = Guid.NewGuid();
            EmploymentId = employmentId;
            QuestionnaireVersion = questionnaireVersion;
            AssessmentDate = assessmentDate;
            CreatedAt = DateTimeOffset.UtcNow;
        }
    }
}
