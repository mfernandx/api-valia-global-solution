using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ValiaApiWeb.Data;
using ValiaApiWeb.Domain.Models;
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
        private readonly ValiaDbContext _context;
        private const string OperationName = "POST:/api/WorkOccurrence";

        public WorkOccurrenceController(IWorkOccurrenceService occurrenceService, ValiaDbContext context)
        {
            _occurrenceService = occurrenceService;
            _context = context;
        }

        private Guid GetAuthenticatedUserId()
        {
            string? claim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (!Guid.TryParse(claim, out Guid userId) || userId == Guid.Empty)
                throw new UnauthorizedAccessException("Não foi possível identificar o usuário autenticado.");
            return userId;
        }

        [HttpPost]
        public async Task<ActionResult<WorkOccurrenceResponse>> Create(
            [FromBody] CreateWorkOccurrenceRequest request,
            [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey)
        {
            Guid userId = GetAuthenticatedUserId();

            if (string.IsNullOrWhiteSpace(idempotencyKey))
                return BadRequest(new { message = "O cabeçalho Idempotency-Key é obrigatório." });

            idempotencyKey = idempotencyKey.Trim();
            if (idempotencyKey.Length > 100)
                return BadRequest(new { message = "A chave não pode ultrapassar 100 caracteres." });

            string requestJson = JsonSerializer.Serialize(request);
            string requestHash = Convert.ToHexString(
                SHA256.HashData(Encoding.UTF8.GetBytes(requestJson)));

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var previous = await _context.IdempotencyRecords.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.UserId == userId &&
                        x.Operation == OperationName && x.IdempotencyKey == idempotencyKey);

                if (previous != null)
                {
                    await transaction.RollbackAsync();
                    return ReplayOrConflict(previous, requestHash);
                }

                WorkOccurrenceResponse response =
                    await _occurrenceService.CreateAsync(userId, request);

                var record = new IdempotencyRecord(
                    userId, OperationName, idempotencyKey, requestHash,
                    JsonSerializer.Serialize(response), StatusCodes.Status201Created);

                await _context.IdempotencyRecords.AddAsync(record);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetById),
                    new { id = response.WorkOccurrenceId }, response);
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();
                _context.ChangeTracker.Clear();

                var previous = await _context.IdempotencyRecords.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.UserId == userId &&
                        x.Operation == OperationName && x.IdempotencyKey == idempotencyKey);

                if (previous == null) throw;
                return ReplayOrConflict(previous, requestHash);
            }
        }

        private ActionResult<WorkOccurrenceResponse> ReplayOrConflict(
            IdempotencyRecord previous, string requestHash)
        {
            if (previous.RequestHash != requestHash)
                return Conflict(new { message = "Esta chave já foi utilizada com dados diferentes." });

            WorkOccurrenceResponse? response =
                JsonSerializer.Deserialize<WorkOccurrenceResponse>(previous.ResponseBody);

            if (response == null)
                throw new InvalidOperationException("Não foi possível recuperar a resposta anterior.");

            return CreatedAtAction(nameof(GetById),
                new { id = response.WorkOccurrenceId }, response);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<WorkOccurrenceResponse>> GetById(Guid id)
        {
            Guid userId = GetAuthenticatedUserId();
            WorkOccurrenceResponse? response = await _occurrenceService.GetByIdAsync(userId, id);
            if (response == null)
                return Problem(statusCode: 404, title: "Ocorrência profissional não encontrada.");
            return Ok(response);
        }

        [HttpGet("employment/{employmentId:guid}")]
        public async Task<ActionResult<IEnumerable<WorkOccurrenceResponse>>> GetByEmploymentId(Guid employmentId)
        {
            Guid userId = GetAuthenticatedUserId();
            IEnumerable<WorkOccurrenceResponse> response =
                await _occurrenceService.GetByEmploymentIdAsync(userId, employmentId);
            return Ok(response);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<WorkOccurrenceResponse>> Update(Guid id, [FromBody] UpdateWorkOccurrenceRequest request)
        {
            Guid userId = GetAuthenticatedUserId();
            WorkOccurrenceResponse response = await _occurrenceService.UpdateAsync(userId, id, request);
            return Ok(response);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            Guid userId = GetAuthenticatedUserId();
            await _occurrenceService.DeleteAsync(userId, id);
            return NoContent();
        }
    }
}
