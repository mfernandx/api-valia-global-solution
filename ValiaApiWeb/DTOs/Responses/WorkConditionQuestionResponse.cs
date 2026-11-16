
using ValiaApiWeb.Domain.Enums;

namespace ValiaApiWeb.DTOs.Responses
{
    public class WorkConditionQuestionResponse
    {
        public Guid WorkConditionQuestionId { get; set; } // Identificador da pergunta
        public string QuestionText { get; set; } = string.Empty; // Texto apresentado ao trabalhador
        public EnumWorkConditionCategory Category { get; set; } // Categoria da pergunta
        public int DisplayOrder { get; set; } // Ordem de apresentação
        public int QuestionnaireVersion { get; set; } // Versão do questionário
    }
}
