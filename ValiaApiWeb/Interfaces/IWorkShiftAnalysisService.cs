using ValiaApiWeb.DTOs.Responses;

namespace ValiaApiWeb.Services.Interfaces
{
    public interface IWorkShiftAnalysisService
    {
        Task<WorkShiftAnalysisResponse> AnalyzeAsync(Guid userId, Guid employmentId, DateOnly startDate, DateOnly endDate);
    }
}

