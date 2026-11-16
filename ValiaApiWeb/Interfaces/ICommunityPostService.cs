using ValiaApiWeb.DTOs.Requests;
using ValiaApiWeb.DTOs.Responses;

namespace ValiaApiWeb.Services.Interfaces
{
    public interface ICommunityPostService
    {
        Task<CommunityPostResponse> CreateAsync(Guid userId, CreateCommunityPostRequest request);
        Task<CommunityPostResponse?> GetByIdAsync(Guid userId, Guid postId);
        Task<IEnumerable<CommunityPostResponse>> GetAllAsync(Guid userId);
        Task<CommunityPostResponse> UpdateAsync(Guid userId, Guid postId, UpdateCommunityPostRequest request);
        Task DeleteAsync(Guid userId, Guid postId);
    }
}

