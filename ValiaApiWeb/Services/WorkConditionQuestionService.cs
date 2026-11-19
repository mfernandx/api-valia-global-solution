using Microsoft.EntityFrameworkCore;
using ValiaApiWeb.Data;
using ValiaApiWeb.DTOs.Responses;
using ValiaApiWeb.Services.Interfaces;

namespace ValiaApiWeb.Services
{
    public class WorkConditionQuestionService : IWorkConditionQuestionService
    {
        private readonly ValiaDbContext _context;

        public WorkConditionQuestionService(ValiaDbContext context)
        {
            _context = context;
        }

        // Recupera as perguntas ativas da versão mais recente
        public async Task<IEnumerable<WorkConditionQuestionResponse>> GetActiveQuestionsAsync()
        {
            int? currentVersion = await _context.WorkConditionQuestions
                .Where(q => q.IsActive)
                .MaxAsync(q => (int?)q.QuestionnaireVersion);

            if (!currentVersion.HasValue)
                return new List<WorkConditionQuestionResponse>();

            return await _context.WorkConditionQuestions
                .AsNoTracking()
                .Where(q => q.IsActive && q.QuestionnaireVersion == currentVersion.Value)
                .OrderBy(q => q.DisplayOrder)
                .Select(q => new WorkConditionQuestionResponse
                {
                    WorkConditionQuestionId = q.WorkConditionQuestionId,
                    QuestionText = q.QuestionText,
                    Category = q.Category,
                    DisplayOrder = q.DisplayOrder,
                    QuestionnaireVersion = q.QuestionnaireVersion
                })
                .ToListAsync();
        }
    }
}

