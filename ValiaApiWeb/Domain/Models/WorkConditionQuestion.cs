
using ValiaApiWeb.Domain.Enums;

namespace ValiaApiWeb.Domain.Models
{
    public class WorkConditionQuestion
    {
        public Guid WorkConditionQuestionId { get; private set; } // Identificador da pergunta
        public string QuestionText { get; private set; } = string.Empty; // Texto apresentado ao trabalhador
        public EnumWorkConditionCategory Category { get; private set; } // Categoria da pergunta
        public int DisplayOrder { get; private set; } // Ordem de apresentação no questionário
        public int QuestionnaireVersion { get; private set; } // Versão do questionário
        public bool IsActive { get; private set; } // Indica se a pergunta está disponível
        public DateTimeOffset CreatedAt { get; private set; } // Data de criação
        public ICollection<WorkConditionAnswer> Answers { get; private set; } = new List<WorkConditionAnswer>(); // Respostas vinculadas à pergunta
        
        protected WorkConditionQuestion() { }

        public WorkConditionQuestion(string questionText, EnumWorkConditionCategory category, int displayOrder, int questionnaireVersion)
        {
            if (string.IsNullOrWhiteSpace(questionText))
                throw new ArgumentException("O texto da pergunta é obrigatório.");

            if (!Enum.IsDefined(category))
                throw new ArgumentException("Categoria inválida.");

            if (displayOrder <= 0 || questionnaireVersion <= 0)
                throw new ArgumentException("A ordem e a versão devem ser positivas.");

            WorkConditionQuestionId = Guid.NewGuid();
            QuestionText = questionText.Trim();
            Category = category;
            DisplayOrder = displayOrder;
            QuestionnaireVersion = questionnaireVersion;
            IsActive = true;
            CreatedAt = DateTimeOffset.UtcNow;
        }
    }
}

