
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using ValiaApiWeb.DTOs.Requests;
using ValiaApiWeb.DTOs.Responses;
using ValiaApiWeb.Services.Interfaces;

namespace ValiaApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class EvidenceController : ControllerBase
    {
        private readonly IEvidenceService _evidenceService;

        public EvidenceController(IEvidenceService evidenceService)
        {
            _evidenceService = evidenceService;
        }

        private Guid GetAuthenticatedUserId()
        {
            string? userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (!Guid.TryParse(userIdClaim, out Guid userId) || userId == Guid.Empty)
                throw new UnauthorizedAccessException("Não foi possível identificar o usuário autenticado.");

            return userId;
        }

        // POST: api/Evidence
        [HttpPost]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(11 * 1024 * 1024)]
        public async Task<ActionResult<EvidenceResponse>> Upload([FromForm] UploadEvidenceRequest request)
        {
            Guid userId = GetAuthenticatedUserId();
            EvidenceResponse response = await _evidenceService.UploadAsync(userId, request);

            return CreatedAtAction(nameof(GetById), new { id = response.EvidenceId }, response);
        }

        // GET: api/Evidence
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EvidenceResponse>>> GetAll()
        {
            Guid userId = GetAuthenticatedUserId();
            IEnumerable<EvidenceResponse> response = await _evidenceService.GetByUserIdAsync(userId);

            return Ok(response);
        }

        // GET: api/Evidence/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EvidenceResponse>> GetById(Guid id)
        {
            Guid userId = GetAuthenticatedUserId();
            EvidenceResponse? response = await _evidenceService.GetByIdAsync(userId, id);

            if (response == null)
                return Problem(statusCode: 404, title: "Documento não encontrado.");

            return Ok(response);
        }

        // GET: api/Evidence/{id}/download
        [HttpGet("{id:guid}/download")]
        public async Task<IActionResult> Download(Guid id)
        {
            Guid userId = GetAuthenticatedUserId();
            EvidenceFileResponse response = await _evidenceService.DownloadAsync(userId, id);

            return File(response.Content, response.ContentType, response.FileName);
        }

        // DELETE: api/Evidence/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            Guid userId = GetAuthenticatedUserId();
            await _evidenceService.DeleteAsync(userId, id);

            return NoContent();
        }
    }
}

