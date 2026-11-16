using ValiaApiWeb.DTOs.Requests;
using ValiaApiWeb.DTOs.Responses;

namespace ValiaApiWeb.Services.Interfaces
{
    public interface IWorkOccurrenceService
    {
        Task<WorkOccurrenceResponse> CreateAsync(Guid userId, CreateWorkOccurrenceRequest request);
        Task<WorkOccurrenceResponse?> GetByIdAsync(Guid userId, Guid occurrenceId);
        Task<IEnumerable<WorkOccurrenceResponse>> GetByEmploymentIdAsync(Guid userId, Guid employmentId);
        Task<WorkOccurrenceResponse> UpdateAsync(Guid userId, Guid occurrenceId, UpdateWorkOccurrenceRequest request);
        Task DeleteAsync(Guid userId, Guid occurrenceId);
    }
}

