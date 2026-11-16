
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
    public class CommunityPostController : ControllerBase
    {
        private readonly ICommunityPostService _postService;

        public CommunityPostController(ICommunityPostService postService)
        {
            _postService = postService;
        }

        private Guid GetAuthenticatedUserId()
        {
            string? claim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (!Guid.TryParse(claim, out Guid userId) || userId == Guid.Empty)
                throw new UnauthorizedAccessException("Usuário autenticado inválido.");

            return userId;
        }

        [HttpPost]
        public async Task<ActionResult<CommunityPostResponse>> Create([FromBody] CreateCommunityPostRequest request)
        {
            Guid userId = GetAuthenticatedUserId();
            CommunityPostResponse response = await _postService.CreateAsync(userId, request);

            return CreatedAtAction(nameof(GetById), new { id = response.CommunityPostId }, response);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommunityPostResponse>>> GetAll()
        {
            Guid userId = GetAuthenticatedUserId();
            return Ok(await _postService.GetAllAsync(userId));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CommunityPostResponse>> GetById(Guid id)
        {
            Guid userId = GetAuthenticatedUserId();
            CommunityPostResponse? response = await _postService.GetByIdAsync(userId, id);

            if (response == null)
                return Problem(statusCode: 404, title: "Publicação não encontrada.");

            return Ok(response);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<CommunityPostResponse>> Update(Guid id, [FromBody] UpdateCommunityPostRequest request)
        {
            Guid userId = GetAuthenticatedUserId();
            return Ok(await _postService.UpdateAsync(userId, id, request));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            Guid userId = GetAuthenticatedUserId();
            await _postService.DeleteAsync(userId, id);

            return NoContent();
        }
    }
}

