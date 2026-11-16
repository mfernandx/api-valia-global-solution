using ValiaApiWeb.Domain.Enums;

namespace ValiaApiWeb.DTOs.Responses
{
    public class WorkConditionCategoryScoreResponse
    {
        public EnumWorkConditionCategory Category { get; set; } // Categoria avaliada
        public decimal AverageScore { get; set; } // Média das respostas
        public int AnswerCount { get; set; } // Quantidade de respostas
    }
}

