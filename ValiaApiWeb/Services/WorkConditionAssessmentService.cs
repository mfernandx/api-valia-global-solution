using Microsoft.EntityFrameworkCore;
using ValiaApiWeb.Data;
using ValiaApiWeb.Domain.Models;
using ValiaApiWeb.DTOs.Requests;
using ValiaApiWeb.DTOs.Responses;
using ValiaApiWeb.Services.Interfaces;

namespace ValiaApiWeb.Services
{
    public class WorkConditionAssessmentService : IWorkConditionAssessmentService
    {
        private readonly ValiaDbContext _context;

        public WorkConditionAssessmentService(ValiaDbContext context)
        {
            _context = context;
        }

        // Registra uma nova avaliação
        public async Task<WorkConditionAssessmentResponse> CreateAsync(Guid userId, CreateWorkConditionAssessmentRequest request)
        {
            ValidateUserId(userId);
            ArgumentNullException.ThrowIfNull(request);

            if (!request.EmploymentId.HasValue || request.EmploymentId.Value == Guid.Empty)
                throw new ArgumentException("O vínculo profissional é obrigatório.");

            if (!request.AssessmentDate.HasValue)
                throw new ArgumentException("A data da avaliação é obrigatória.");

            if (request.AssessmentDate.Value > DateOnly.FromDateTime(DateTime.UtcNow))
                throw new ArgumentException("A data da avaliação não pode ser futura.");

            bool employmentExists = await _context.Employments
                .AnyAsync(e => e.EmploymentId == request.EmploymentId.Value && e.UserId == userId && e.User.IsActive);

            if (!employmentExists)
                throw new KeyNotFoundException("Vínculo profissional não encontrado.");

            int? currentVersion = await _context.WorkConditionQuestions
                .Where(q => q.IsActive)
                .MaxAsync(q => (int?)q.QuestionnaireVersion);

            if (!currentVersion.HasValue)
                throw new InvalidOperationException("Nenhum questionário ativo foi configurado.");

            List<WorkConditionQuestion> questions = await _context.WorkConditionQuestions
                .AsNoTracking()
                .Where(q => q.IsActive && q.QuestionnaireVersion == currentVersion.Value)
                .ToListAsync();

            ValidateAnswers(request.Answers, questions);

            var assessment = new WorkConditionAssessment(
                request.EmploymentId.Value, currentVersion.Value, request.AssessmentDate.Value);

            await _context.WorkConditionAssessments.AddAsync(assessment);

            foreach (CreateWorkConditionAnswerRequest answerRequest in request.Answers)
            {
                var answer = new WorkConditionAnswer(
                    assessment.WorkConditionAssessmentId,
                    answerRequest.WorkConditionQuestionId!.Value,
                    answerRequest.Score!.Value,
                    answerRequest.Observation
                );

                await _context.WorkConditionAnswers.AddAsync(answer);
            }

            await _context.SaveChangesAsync();

            return await GetAssessmentResponseAsync(assessment.WorkConditionAssessmentId);
        }

        // Consulta uma avaliação específica
        public async Task<WorkConditionAssessmentResponse?> GetByIdAsync(Guid userId, Guid assessmentId)
        {
            ValidateUserId(userId);

            if (assessmentId == Guid.Empty)
                throw new ArgumentException("Identificador da avaliação inválido.");

            bool exists = await _context.WorkConditionAssessments
                .AnyAsync(a => a.WorkConditionAssessmentId == assessmentId && a.Employment.UserId == userId);

            return exists ? await GetAssessmentResponseAsync(assessmentId) : null;
        }

        // Lista as avaliações de um vínculo profissional
        public async Task<IEnumerable<WorkConditionAssessmentResponse>> GetByEmploymentIdAsync(Guid userId, Guid employmentId)
        {
            ValidateUserId(userId);

            if (employmentId == Guid.Empty)
                throw new ArgumentException("Identificador do vínculo profissional inválido.");

            bool employmentExists = await _context.Employments
                .AnyAsync(e => e.EmploymentId == employmentId && e.UserId == userId);

            if (!employmentExists)
                throw new KeyNotFoundException("Vínculo profissional não encontrado.");

            List<Guid> assessmentIds = await _context.WorkConditionAssessments
                .AsNoTracking()
                .Where(a => a.EmploymentId == employmentId)
                .OrderByDescending(a => a.AssessmentDate)
                .Select(a => a.WorkConditionAssessmentId)
                .ToListAsync();

            var responses = new List<WorkConditionAssessmentResponse>();

            foreach (Guid assessmentId in assessmentIds)
                responses.Add(await GetAssessmentResponseAsync(assessmentId));

            return responses;
        }

        // Valida se todas as perguntas foram respondidas corretamente
        private static void ValidateAnswers(
            List<CreateWorkConditionAnswerRequest> answers, List<WorkConditionQuestion> questions)
        {
            if (answers == null || answers.Count == 0)
                throw new ArgumentException("É necessário informar as respostas do questionário.");

            if (answers.Any(a => a == null || !a.WorkConditionQuestionId.HasValue ||
                a.WorkConditionQuestionId.Value == Guid.Empty || !a.Score.HasValue ||
                a.Score.Value < 1 || a.Score.Value > 5 ||
                (a.Observation != null && a.Observation.Length > 1000)))
                throw new ArgumentException("Uma ou mais respostas possuem dados inválidos.");

            List<Guid> questionIds = answers.Select(a => a.WorkConditionQuestionId!.Value).ToList();

            if (questionIds.Distinct().Count() != questionIds.Count)
                throw new ArgumentException("O questionário contém respostas duplicadas.");

            HashSet<Guid> expectedIds = questions.Select(q => q.WorkConditionQuestionId).ToHashSet();

            if (questionIds.Count != expectedIds.Count || questionIds.Any(id => !expectedIds.Contains(id)))
                throw new ArgumentException("Todas as perguntas ativas do questionário devem ser respondidas.");
        }

        // Recupera a avaliação e calcula seus indicadores
        private async Task<WorkConditionAssessmentResponse> GetAssessmentResponseAsync(Guid assessmentId)
        {
            WorkConditionAssessment assessment = await _context.WorkConditionAssessments
                .AsNoTracking()
                .Include(a => a.Answers)
                .ThenInclude(a => a.Question)
                .FirstAsync(a => a.WorkConditionAssessmentId == assessmentId);

            List<WorkConditionCategoryScoreResponse> categoryScores = assessment.Answers
                .GroupBy(a => a.Question.Category)
                .Select(group => new WorkConditionCategoryScoreResponse
                {
                    Category = group.Key,
                    AverageScore = Math.Round(group.Average(a => (decimal)a.Score), 2),
                    AnswerCount = group.Count()
                })
                .OrderBy(c => c.Category)
                .ToList();

            decimal averageScore = assessment.Answers.Count == 0 ? 0 :
                Math.Round(assessment.Answers.Average(a => (decimal)a.Score), 2);

            return new WorkConditionAssessmentResponse
            {
                WorkConditionAssessmentId = assessment.WorkConditionAssessmentId,
                EmploymentId = assessment.EmploymentId,
                QuestionnaireVersion = assessment.QuestionnaireVersion,
                AssessmentDate = assessment.AssessmentDate,
                CreatedAt = assessment.CreatedAt,
                AverageScore = averageScore,
                CategoryScores = categoryScores
            };
        }

        private static void ValidateUserId(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("Identificador do usuário inválido.");
        }
    }
}

