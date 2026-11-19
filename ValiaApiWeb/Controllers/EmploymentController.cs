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
    public class EmploymentController : ControllerBase
    {
        private readonly IEmploymentService _employmentService;

        public EmploymentController(IEmploymentService employmentService)
        {
            _employmentService = employmentService;
        }

        private Guid GetAuthenticatedUserId()
        {
            string? userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (!Guid.TryParse(userIdClaim, out Guid userId) || userId == Guid.Empty)
            {
                throw new UnauthorizedAccessException("Não foi possível identificar o usuário autenticado.");
            }

            return userId;
        }

       
        [HttpPost]
        public async Task<ActionResult<EmploymentResponse>> Create([FromBody] CreateEmploymentRequest request)
        {
            Guid userId = GetAuthenticatedUserId();

            EmploymentResponse response = await _employmentService.CreateAsync(userId, request);

            return CreatedAtAction(nameof(GetById), new { id = response.EmploymentId }, response);
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmploymentResponse>>> GetAll()
        {
            Guid userId = GetAuthenticatedUserId();

            IEnumerable<EmploymentResponse> response = await _employmentService.GetByUserIdAsync(userId);

            return Ok(response);
        }


        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EmploymentResponse>> GetById(Guid id)
        {
            Guid userId = GetAuthenticatedUserId();

            EmploymentResponse? response = await _employmentService.GetByIdAsync(userId, id);

            if (response == null)
                return NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Vínculo profissional não encontrado.",
                    Detail = "O vínculo solicitado não existe ou não pertence ao usuário autenticado."
                });

            return Ok(response);
        }


        [HttpPut("{id:guid}")]
        public async Task<ActionResult<EmploymentResponse>> Update(Guid id, [FromBody] UpdateEmploymentRequest request)
        {
            Guid userId = GetAuthenticatedUserId();

            EmploymentResponse response = await _employmentService.UpdateAsync(userId, id, request);

            return Ok(response);
        }


        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            Guid userId = GetAuthenticatedUserId();

            await _employmentService.DeleteAsync(userId, id);

            return NoContent();
        }
    }
}

