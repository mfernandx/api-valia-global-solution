
using Microsoft.EntityFrameworkCore;
using ValiaApiWeb.Data;
using ValiaApiWeb.Domain.Models;
using ValiaApiWeb.Domain.Enums;
using ValiaApiWeb.DTOs.Requests;
using ValiaApiWeb.DTOs.Responses;
using ValiaApiWeb.Services.Interfaces;

namespace ValiaApi.Services
{
    public class CommunityPostService : ICommunityPostService
    {
        private readonly ValiaDbContext _context;

        public CommunityPostService(ValiaDbContext context)
        {
            _context = context;
        }

        public async Task<CommunityPostResponse> CreateAsync(Guid userId, CreateCommunityPostRequest request)
        {
            ValidateUserId(userId);
            ArgumentNullException.ThrowIfNull(request);
            ValidatePostData(request.Category, request.Title, request.Content);

            bool activeUserExists = await _context.Users.AnyAsync(u => u.UserId == userId && u.IsActive);

            if (!activeUserExists)
                throw new KeyNotFoundException("Usuário ativo não encontrado.");

            var post = new CommunityPost(userId, request.Category!.Value, request.Title, request.Content, request.IsAnonymous);

            await _context.CommunityPosts.AddAsync(post);
            await _context.SaveChangesAsync();

            return await GetPostResponseAsync(post.CommunityPostId, userId);
        }

        public async Task<CommunityPostResponse?> GetByIdAsync(Guid userId, Guid postId)
        {
            ValidateUserId(userId);
            ValidatePostId(postId);

            bool exists = await _context.CommunityPosts.AsNoTracking()
                .AnyAsync(p => p.CommunityPostId == postId && p.IsActive);

            return exists ? await GetPostResponseAsync(postId, userId) : null;
        }

        public async Task<IEnumerable<CommunityPostResponse>> GetAllAsync(Guid userId)
        {
            ValidateUserId(userId);

            return await _context.CommunityPosts.AsNoTracking()
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new CommunityPostResponse
                {
                    CommunityPostId = p.CommunityPostId,
                    UserId = p.IsAnonymous ? null : p.UserId,
                    AuthorName = p.IsAnonymous ? "Anônimo" : p.User.FullName,
                    Category = p.Category,
                    Title = p.Title,
                    Content = p.Content,
                    IsAnonymous = p.IsAnonymous,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    Version = p.Version,
                    CommentCount = p.Comments.Count(c => c.IsActive),
                    IsOwner = p.UserId == userId
                })
                .ToListAsync();
        }

        public async Task<CommunityPostResponse> UpdateAsync(Guid userId, Guid postId, UpdateCommunityPostRequest request)
        {
            ValidateUserId(userId);
            ValidatePostId(postId);
            ArgumentNullException.ThrowIfNull(request);
            ValidatePostData(request.Category, request.Title, request.Content);

            CommunityPost post = await _context.CommunityPosts
                .FirstOrDefaultAsync(p => p.CommunityPostId == postId && p.UserId == userId && p.IsActive)
                ?? throw new KeyNotFoundException("Publicação não encontrada.");

            post.Update(request.Category!.Value, request.Title, request.Content, request.IsAnonymous);

            await _context.SaveChangesAsync();
            return await GetPostResponseAsync(post.CommunityPostId, userId);
        }

        public async Task DeleteAsync(Guid userId, Guid postId)
        {
            ValidateUserId(userId);
            ValidatePostId(postId);

            CommunityPost post = await _context.CommunityPosts
                .FirstOrDefaultAsync(p => p.CommunityPostId == postId && p.UserId == userId && p.IsActive)
                ?? throw new KeyNotFoundException("Publicação não encontrada.");

            post.Deactivate();
            await _context.SaveChangesAsync();
        }

        private async Task<CommunityPostResponse> GetPostResponseAsync(Guid postId, Guid userId)
        {
            return await _context.CommunityPosts.AsNoTracking()
                .Where(p => p.CommunityPostId == postId && p.IsActive)
                .Select(p => new CommunityPostResponse
                {
                    CommunityPostId = p.CommunityPostId,
                    UserId = p.IsAnonymous ? null : p.UserId,
                    AuthorName = p.IsAnonymous ? "Anônimo" : p.User.FullName,
                    Category = p.Category,
                    Title = p.Title,
                    Content = p.Content,
                    IsAnonymous = p.IsAnonymous,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    Version = p.Version,
                    CommentCount = p.Comments.Count(c => c.IsActive),
                    IsOwner = p.UserId == userId
                })
                .FirstAsync();
        }

        private static void ValidatePostData(EnumCommunityPostCategory? category, string title, string content)
        {
            if (!category.HasValue || !Enum.IsDefined(category.Value))
                throw new ArgumentException("Categoria da publicação inválida.");

            if (string.IsNullOrWhiteSpace(title) || title.Length > 150)
                throw new ArgumentException("O título deve conter entre 1 e 150 caracteres.");

            if (string.IsNullOrWhiteSpace(content) || content.Length > 5000)
                throw new ArgumentException("O conteúdo deve conter entre 1 e 5000 caracteres.");
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
    }
}

