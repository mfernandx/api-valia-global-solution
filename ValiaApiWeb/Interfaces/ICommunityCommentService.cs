
using ValiaApiWeb.DTOs.Requests;
using ValiaApiWeb.DTOs.Responses;

namespace ValiaApiWeb.Services.Interfaces
{
    public interface ICommunityCommentService
    {
        Task<CommunityCommentResponse> CreateAsync(Guid userId, CreateCommunityCommentRequest request);
        Task<IEnumerable<CommunityCommentResponse>> GetByPostIdAsync(Guid userId, Guid postId);
        Task<CommunityCommentResponse> UpdateAsync(Guid userId, Guid commentId, UpdateCommunityCommentRequest request);
        Task DeleteAsync(Guid userId, Guid commentId);
    }
}

