
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
    public class WorkShiftController : ControllerBase
    {
        private readonly IWorkShiftService _workShiftService;

        public WorkShiftController(IWorkShiftService workShiftService)
        {
            _workShiftService = workShiftService;
        }

        
        private Guid GetAuthenticatedUserId()
        {
            string? userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (!Guid.TryParse(userIdClaim, out Guid userId) || userId == Guid.Empty)
                throw new UnauthorizedAccessException("Não foi possível identificar o usuário autenticado.");

            return userId;
        }

        
        [HttpPost]
        public async Task<ActionResult<WorkShiftResponse>> Create([FromBody] CreateWorkShiftRequest request)
        {
            Guid userId = GetAuthenticatedUserId();
            WorkShiftResponse response = await _workShiftService.CreateAsync(userId, request);

            return CreatedAtAction(nameof(GetById), new { id = response.WorkShiftId }, response);
        }

        
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<WorkShiftResponse>> GetById(Guid id)
        {
            Guid userId = GetAuthenticatedUserId();
            WorkShiftResponse? response = await _workShiftService.GetByIdAsync(userId, id);

            if (response == null)
                return Problem(statusCode: 404, title: "Jornada não encontrada.");

            return Ok(response);
        }

        
        [HttpGet("employment/{employmentId:guid}")]
        public async Task<ActionResult<IEnumerable<WorkShiftResponse>>> GetByEmploymentId(Guid employmentId)
        {
            Guid userId = GetAuthenticatedUserId();
            IEnumerable<WorkShiftResponse> response = await _workShiftService.GetByEmploymentIdAsync(userId, employmentId);

            return Ok(response);
        }

        
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<WorkShiftResponse>> Update(Guid id, [FromBody] UpdateWorkShiftRequest request)
        {
            Guid userId = GetAuthenticatedUserId();
            WorkShiftResponse response = await _workShiftService.UpdateAsync(userId, id, request);

            return Ok(response);
        }

      
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            Guid userId = GetAuthenticatedUserId();
            await _workShiftService.DeleteAsync(userId, id);

            return NoContent();
        }
    }
}

