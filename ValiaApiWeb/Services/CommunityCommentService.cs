using Microsoft.EntityFrameworkCore;
using ValiaApiWeb.Data;
using ValiaApiWeb.Domain.Models;
using ValiaApiWeb.DTOs.Requests;
using ValiaApiWeb.DTOs.Responses;
using ValiaApiWeb.Services.Interfaces;

namespace ValiaApi.Services
{
    public class CommunityCommentService : ICommunityCommentService
    {
        private readonly ValiaDbContext _context;

        public CommunityCommentService(ValiaDbContext context)
        {
            _context = context;
        }

        public async Task<CommunityCommentResponse> CreateAsync(Guid userId, CreateCommunityCommentRequest request)
        {
            ValidateUserId(userId);
            ArgumentNullException.ThrowIfNull(request);

            if (!request.CommunityPostId.HasValue || request.CommunityPostId.Value == Guid.Empty)
                throw new ArgumentException("A publicação é obrigatória.");

            ValidateContent(request.Content);

            bool activeUserExists = await _context.Users.AnyAsync(u => u.UserId == userId && u.IsActive);

            if (!activeUserExists)
                throw new KeyNotFoundException("Usuário ativo não encontrado.");

            bool postExists = await _context.CommunityPosts
                .AnyAsync(p => p.CommunityPostId == request.CommunityPostId.Value && p.IsActive);

            if (!postExists)
                throw new KeyNotFoundException("Publicação não encontrada.");

            var comment = new CommunityComment(request.CommunityPostId.Value, userId, request.Content, request.IsAnonymous);

            await _context.CommunityComments.AddAsync(comment);
            await _context.SaveChangesAsync();

            return await GetCommentResponseAsync(comment.CommunityCommentId, userId);
        }

        public async Task<IEnumerable<CommunityCommentResponse>> GetByPostIdAsync(Guid userId, Guid postId)
        {
            ValidateUserId(userId);
            ValidatePostId(postId);

            bool postExists = await _context.CommunityPosts
                .AnyAsync(p => p.CommunityPostId == postId && p.IsActive);

            if (!postExists)
                throw new KeyNotFoundException("Publicação não encontrada.");

            return await _context.CommunityComments.AsNoTracking()
                .Where(c => c.CommunityPostId == postId && c.IsActive)
                .OrderBy(c => c.CreatedAt)
                .Select(c => new CommunityCommentResponse
                {
                    CommunityCommentId = c.CommunityCommentId,
                    CommunityPostId = c.CommunityPostId,
                    UserId = c.IsAnonymous ? null : c.UserId,
                    AuthorName = c.IsAnonymous ? "Anônimo" : c.User.FullName,
                    Content = c.Content,
                    IsAnonymous = c.IsAnonymous,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    Version = c.Version,
                    IsOwner = c.UserId == userId
                })
                .ToListAsync();
        }

        public async Task<CommunityCommentResponse> UpdateAsync(Guid userId, Guid commentId, UpdateCommunityCommentRequest request)
        {
            ValidateUserId(userId);
            ValidateCommentId(commentId);
            ArgumentNullException.ThrowIfNull(request);
            ValidateContent(request.Content);

            CommunityComment comment = await _context.CommunityComments
                .FirstOrDefaultAsync(c => c.CommunityCommentId == commentId && c.UserId == userId &&
                    c.IsActive && c.CommunityPost.IsActive)
                ?? throw new KeyNotFoundException("Comentário não encontrado.");

            comment.UpdateContent(request.Content);
            await _context.SaveChangesAsync();

            return await GetCommentResponseAsync(comment.CommunityCommentId, userId);
        }

        public async Task DeleteAsync(Guid userId, Guid commentId)
        {
            ValidateUserId(userId);
            ValidateCommentId(commentId);

            CommunityComment comment = await _context.CommunityComments
                .FirstOrDefaultAsync(c => c.CommunityCommentId == commentId && c.UserId == userId && c.IsActive)
                ?? throw new KeyNotFoundException("Comentário não encontrado.");

            comment.Deactivate();
            await _context.SaveChangesAsync();
        }

        private async Task<CommunityCommentResponse> GetCommentResponseAsync(Guid commentId, Guid userId)
        {
            return await _context.CommunityComments.AsNoTracking()
                .Where(c => c.CommunityCommentId == commentId && c.IsActive)
                .Select(c => new CommunityCommentResponse
                {
                    CommunityCommentId = c.CommunityCommentId,
                    CommunityPostId = c.CommunityPostId,
                    UserId = c.IsAnonymous ? null : c.UserId,
                    AuthorName = c.IsAnonymous ? "Anônimo" : c.User.FullName,
                    Content = c.Content,
                    IsAnonymous = c.IsAnonymous,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    Version = c.Version,
                    IsOwner = c.UserId == userId
                })
                .FirstAsync();
        }

        private static void ValidateContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content) || content.Length > 2000)
                throw new ArgumentException("O comentário deve conter entre 1 e 2000 caracteres.");
        }

        private static void ValidateUserId(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("Identificador do usuário inválido.");
        }

        private static void ValidatePostId(Guid postId)
        {
            if (postId == Guid.Empty)
                throw new ArgumentException("Identificador da publicação inválido.");
        }

        private static void ValidateCommentId(Guid commentId)
        {
            if (commentId == Guid.Empty)
                throw new ArgumentException("Identificador do comentário inválido.");
        }
    }
}

