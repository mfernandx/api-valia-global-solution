
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
    public class WorkOccurrenceController : ControllerBase
    {
        private readonly IWorkOccurrenceService _occurrenceService;

        public WorkOccurrenceController(IWorkOccurrenceService occurrenceService)
        {
            _occurrenceService = occurrenceService;
        }

        // Obtém o usuário autenticado pelo JWT
        private Guid GetAuthenticatedUserId()
        {
            string? userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (!Guid.TryParse(userIdClaim, out Guid userId) || userId == Guid.Empty)
                throw new UnauthorizedAccessException("Não foi possível identificar o usuário autenticado.");

            return userId;
        }

        // POST: api/WorkOccurrence
        [HttpPost]
        public async Task<ActionResult<WorkOccurrenceResponse>> Create([FromBody] CreateWorkOccurrenceRequest request)
        {
            Guid userId = GetAuthenticatedUserId();
            WorkOccurrenceResponse response = await _occurrenceService.CreateAsync(userId, request);

            return CreatedAtAction(nameof(GetById), new { id = response.WorkOccurrenceId }, response);
        }

        // GET: api/WorkOccurrence/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<WorkOccurrenceResponse>> GetById(Guid id)
        {
            Guid userId = GetAuthenticatedUserId();
            WorkOccurrenceResponse? response = await _occurrenceService.GetByIdAsync(userId, id);

            if (response == null)
                return Problem(statusCode: 404, title: "Ocorrência profissional não encontrada.");

            return Ok(response);
        }

        // GET: api/WorkOccurrence/employment/{employmentId}
        [HttpGet("employment/{employmentId:guid}")]
        public async Task<ActionResult<IEnumerable<WorkOccurrenceResponse>>> GetByEmploymentId(Guid employmentId)
        {
            Guid userId = GetAuthenticatedUserId();
            IEnumerable<WorkOccurrenceResponse> response =
                await _occurrenceService.GetByEmploymentIdAsync(userId, employmentId);

            return Ok(response);
        }

        // PUT: api/WorkOccurrence/{id}
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<WorkOccurrenceResponse>> Update(Guid id, [FromBody] UpdateWorkOccurrenceRequest request)
        {
            Guid userId = GetAuthenticatedUserId();
            WorkOccurrenceResponse response = await _occurrenceService.UpdateAsync(userId, id, request);

            return Ok(response);
        }

        // DELETE: api/WorkOccurrence/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            Guid userId = GetAuthenticatedUserId();
            await _occurrenceService.DeleteAsync(userId, id);

            return NoContent();
        }
    }
}

