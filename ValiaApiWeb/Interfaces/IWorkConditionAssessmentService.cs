using ValiaApiWeb.DTOs.Requests;
using ValiaApiWeb.DTOs.Responses;

namespace ValiaApiWeb.Services.Interfaces
{
    public interface IWorkConditionAssessmentService
    {
        Task<WorkConditionAssessmentResponse> CreateAsync(Guid userId, CreateWorkConditionAssessmentRequest request);
        Task<WorkConditionAssessmentResponse?> GetByIdAsync(Guid userId, Guid assessmentId);
        Task<IEnumerable<WorkConditionAssessmentResponse>> GetByEmploymentIdAsync(Guid userId, Guid employmentId);
    }
}

