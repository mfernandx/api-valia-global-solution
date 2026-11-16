
using ValiaApiWeb.DTOs.Requests;
using ValiaApiWeb.DTOs.Responses;

namespace ValiaApiWeb.Services.Interfaces
{
    public interface IEmploymentService
    {
        Task<EmploymentResponse> CreateAsync(Guid userId, CreateEmploymentRequest request);
        Task<EmploymentResponse?> GetByIdAsync(Guid userId, Guid employmentId);
        Task<IEnumerable<EmploymentResponse>> GetByUserIdAsync(Guid userId);
        Task<EmploymentResponse> UpdateAsync(Guid userId, Guid employmentId, UpdateEmploymentRequest request);
        Task DeleteAsync(Guid userId, Guid employmentId);
    }
}

