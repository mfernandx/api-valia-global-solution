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
    public class WorkConditionAssessmentController : ControllerBase
    {
        private readonly IWorkConditionAssessmentService _assessmentService;

        public WorkConditionAssessmentController(IWorkConditionAssessmentService assessmentService)
        {
            _assessmentService = assessmentService;
        }

        private Guid GetAuthenticatedUserId()
        {
            string? userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (!Guid.TryParse(userIdClaim, out Guid userId) || userId == Guid.Empty)
                throw new UnauthorizedAccessException("Não foi possível identificar o usuário autenticado.");

            return userId;
        }

        // POST: api/WorkConditionAssessment
        [HttpPost]
        public async Task<ActionResult<WorkConditionAssessmentResponse>> Create([FromBody] CreateWorkConditionAssessmentRequest request)
        {
            Guid userId = GetAuthenticatedUserId();
            WorkConditionAssessmentResponse response = await _assessmentService.CreateAsync(userId, request);

            return CreatedAtAction(nameof(GetById), new { id = response.WorkConditionAssessmentId }, response);
        }

        // GET: api/WorkConditionAssessment/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<WorkConditionAssessmentResponse>> GetById(Guid id)
        {
            Guid userId = GetAuthenticatedUserId();
            WorkConditionAssessmentResponse? response = await _assessmentService.GetByIdAsync(userId, id);

            if (response == null)
                return Problem(statusCode: 404, title: "Avaliação não encontrada.");

            return Ok(response);
        }

        // GET: api/WorkConditionAssessment/employment/{employmentId}
        [HttpGet("employment/{employmentId:guid}")]
        public async Task<ActionResult<IEnumerable<WorkConditionAssessmentResponse>>> GetByEmploymentId(Guid employmentId)
        {
            Guid userId = GetAuthenticatedUserId();
            IEnumerable<WorkConditionAssessmentResponse> response =
                await _assessmentService.GetByEmploymentIdAsync(userId, employmentId);

            return Ok(response);
        }
    }
}

