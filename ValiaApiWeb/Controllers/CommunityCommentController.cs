
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using ValiaApiWeb.DTOs.Requests;
using ValiaApiWeb.DTOs.Responses;
using ValiaApiWeb.Services.Interfaces;

namespace ValiaApiWeb.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class CommunityCommentController : ControllerBase
    {
        private readonly ICommunityCommentService _commentService;

        public CommunityCommentController(ICommunityCommentService commentService)
        {
            _commentService = commentService;
        }

        private Guid GetAuthenticatedUserId()
        {
            string? claim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (!Guid.TryParse(claim, out Guid userId) || userId == Guid.Empty)
                throw new UnauthorizedAccessException("Usuário autenticado inválido.");

            return userId;
        }

        [HttpPost]
        public async Task<ActionResult<CommunityCommentResponse>> Create([FromBody] CreateCommunityCommentRequest request)
        {
            Guid userId = GetAuthenticatedUserId();
            CommunityCommentResponse response = await _commentService.CreateAsync(userId, request);

            return StatusCode(201, response);
        }

        [HttpGet("post/{postId:guid}")]
        public async Task<ActionResult<IEnumerable<CommunityCommentResponse>>> GetByPostId(Guid postId)
        {
            Guid userId = GetAuthenticatedUserId();
            return Ok(await _commentService.GetByPostIdAsync(userId, postId));
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<CommunityCommentResponse>> Update(Guid id, [FromBody] UpdateCommunityCommentRequest request)
        {
            Guid userId = GetAuthenticatedUserId();
            return Ok(await _commentService.UpdateAsync(userId, id, request));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            Guid userId = GetAuthenticatedUserId();
            await _commentService.DeleteAsync(userId, id);

            return NoContent();
        }
    }
}

