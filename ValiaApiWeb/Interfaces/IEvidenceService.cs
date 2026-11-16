using ValiaApiWeb.DTOs.Requests;
using ValiaApiWeb.DTOs.Responses;

namespace ValiaApiWeb.Services.Interfaces
{
    public interface IEvidenceService
    {
        Task<EvidenceResponse> UploadAsync(Guid userId, UploadEvidenceRequest request);
        Task<EvidenceResponse?> GetByIdAsync(Guid userId, Guid evidenceId);
        Task<IEnumerable<EvidenceResponse>> GetByUserIdAsync(Guid userId);
        Task<EvidenceFileResponse> DownloadAsync(Guid userId, Guid evidenceId);
        Task DeleteAsync(Guid userId, Guid evidenceId);
    }
}

