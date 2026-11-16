
using ValiaApiWeb.DTOs.Responses;

namespace ValiaApiWeb.Services.Interfaces
{
    public interface IWorkConditionQuestionService
    {
        Task<IEnumerable<WorkConditionQuestionResponse>> GetActiveQuestionsAsync();
    }
}

