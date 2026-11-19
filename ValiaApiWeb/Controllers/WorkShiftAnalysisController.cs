using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using ValiaApiWeb.DTOs.Responses;
using ValiaApiWeb.Services.Interfaces;

namespace ValiaApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class WorkShiftAnalysisController : ControllerBase
    {
        private readonly IWorkShiftAnalysisService _analysisService;

        public WorkShiftAnalysisController(IWorkShiftAnalysisService analysisService)
        {
            _analysisService = analysisService;
        }

        // Obtém o identificador do usuário autenticado
        private Guid GetAuthenticatedUserId()
        {
            string? userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (!Guid.TryParse(userIdClaim, out Guid userId) || userId == Guid.Empty)
                throw new UnauthorizedAccessException("Não foi possível identificar o usuário autenticado.");

            return userId;
        }

        // GET: api/WorkShiftAnalysis/employment/{employmentId}?startDate=2026-11-01&endDate=2026-11-30
        [HttpGet("employment/{employmentId:guid}")]
        public async Task<ActionResult<WorkShiftAnalysisResponse>> Analyze(
            Guid employmentId, [FromQuery] DateOnly? startDate, [FromQuery] DateOnly? endDate)
        {
            if (!startDate.HasValue || !endDate.HasValue)
                return BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Período de análise obrigatório.",
                    Detail = "Informe startDate e endDate no formato YYYY-MM-DD."
                });

            Guid userId = GetAuthenticatedUserId();

            WorkShiftAnalysisResponse response = await _analysisService.AnalyzeAsync(userId, employmentId, startDate.Value, endDate.Value);

            return Ok(response);
        }
    }
}
