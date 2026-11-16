using ValiaApiWeb.DTOs.Requests;
using ValiaApiWeb.DTOs.Responses;

namespace ValiaApiWeb.Services.Interfaces
{
    public interface IWorkShiftService
    {
        Task<WorkShiftResponse> CreateAsync(Guid userId, CreateWorkShiftRequest request);
        Task<WorkShiftResponse?> GetByIdAsync(Guid userId, Guid workShiftId);
        Task<IEnumerable<WorkShiftResponse>> GetByEmploymentIdAsync(Guid userId,Guid employmentId);
        Task<WorkShiftResponse> UpdateAsync(Guid userId, Guid workShiftId, UpdateWorkShiftRequest request);
        Task DeleteAsync(Guid userId, Guid workShiftId);
    }
}

