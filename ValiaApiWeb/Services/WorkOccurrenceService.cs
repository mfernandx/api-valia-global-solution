
using Microsoft.EntityFrameworkCore;
using ValiaApiWeb.Data;
using ValiaApiWeb.Domain.Enums;
using ValiaApiWeb.Domain.Models;
using ValiaApiWeb.DTOs.Requests;
using ValiaApiWeb.DTOs.Responses;
using ValiaApiWeb.Services.Interfaces;

namespace ValiaApiWeb.Services
{
    public class WorkOccurrenceService : IWorkOccurrenceService
    {
        private readonly ValiaDbContext _context;

        public WorkOccurrenceService(ValiaDbContext context)
        {
            _context = context;
        }

        // Registra uma ocorrência profissional
        public async Task<WorkOccurrenceResponse> CreateAsync(Guid userId, CreateWorkOccurrenceRequest request)
        {
            ValidateUserId(userId);
            ArgumentNullException.ThrowIfNull(request);

            if (!request.EmploymentId.HasValue || request.EmploymentId.Value == Guid.Empty)
                throw new ArgumentException("O vínculo profissional é obrigatório.");

            ValidateOccurrenceData(request.Category, request.Description, request.OccurredAt);

            bool employmentExists = await _context.Employments
                .AnyAsync(e => e.EmploymentId == request.EmploymentId.Value && e.UserId == userId && e.User.IsActive);

            if (!employmentExists)
                throw new KeyNotFoundException("Vínculo profissional não encontrado.");

            var occurrence = new WorkOccurrence(
                request.EmploymentId.Value,
                request.Category!.Value,
                request.Description,
                request.OccurredAt!.Value
            );

            await _context.WorkOccurrences.AddAsync(occurrence);
            await _context.SaveChangesAsync();

            return MapToResponse(occurrence);
        }

        // Consulta uma ocorrência do usuário
        public async Task<WorkOccurrenceResponse?> GetByIdAsync(Guid userId, Guid occurrenceId)
        {
            ValidateUserId(userId);
            ValidateOccurrenceId(occurrenceId);

            WorkOccurrence? occurrence = await _context.WorkOccurrences
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.WorkOccurrenceId == occurrenceId && o.Employment.UserId == userId);

            return occurrence == null ? null : MapToResponse(occurrence);
        }

        // Lista as ocorrências de um vínculo profissional
        public async Task<IEnumerable<WorkOccurrenceResponse>> GetByEmploymentIdAsync(Guid userId, Guid employmentId)
        {
            ValidateUserId(userId);

            if (employmentId == Guid.Empty)
                throw new ArgumentException("Identificador do vínculo profissional inválido.");

            bool employmentExists = await _context.Employments
                .AnyAsync(e => e.EmploymentId == employmentId && e.UserId == userId);

            if (!employmentExists)
                throw new KeyNotFoundException("Vínculo profissional não encontrado.");

            List<WorkOccurrence> occurrences = await _context.WorkOccurrences
                .AsNoTracking()
                .Where(o => o.EmploymentId == employmentId)
                .OrderByDescending(o => o.OccurredAt)
                .ToListAsync();

            return occurrences.Select(MapToResponse).ToList();
        }

        // Atualiza uma ocorrência existente
        public async Task<WorkOccurrenceResponse> UpdateAsync(Guid userId, Guid occurrenceId, UpdateWorkOccurrenceRequest request)
        {
            ValidateUserId(userId);
            ValidateOccurrenceId(occurrenceId);
            ArgumentNullException.ThrowIfNull(request);

            ValidateOccurrenceData(request.Category, request.Description, request.OccurredAt);

            WorkOccurrence occurrence = await _context.WorkOccurrences
                .FirstOrDefaultAsync(o => o.WorkOccurrenceId == occurrenceId && o.Employment.UserId == userId)
                ?? throw new KeyNotFoundException("Ocorrência profissional não encontrada.");

            occurrence.Update(request.Category!.Value, request.Description, request.OccurredAt!.Value);

            await _context.SaveChangesAsync();
            return MapToResponse(occurrence);
        }

        // Exclui uma ocorrência pertencente ao usuário
        public async Task DeleteAsync(Guid userId, Guid occurrenceId)
        {
            ValidateUserId(userId);
            ValidateOccurrenceId(occurrenceId);

            WorkOccurrence occurrence = await _context.WorkOccurrences
                .FirstOrDefaultAsync(o => o.WorkOccurrenceId == occurrenceId && o.Employment.UserId == userId)
                ?? throw new KeyNotFoundException("Ocorrência profissional não encontrada.");

            _context.WorkOccurrences.Remove(occurrence);
            await _context.SaveChangesAsync();
        }

        // Valida os dados obrigatórios da ocorrência
        private static void ValidateOccurrenceData(
            OccurrenceCategory? category, string description, DateTimeOffset? occurredAt)
        {
            if (!category.HasValue || !Enum.IsDefined(category.Value))
                throw new ArgumentException("A categoria da ocorrência é inválida.");

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("A descrição da ocorrência é obrigatória.");

            if (description.Length > 3000)
                throw new ArgumentException("A descrição não pode ultrapassar 3000 caracteres.");

            if (!occurredAt.HasValue)
                throw new ArgumentException("A data da ocorrência é obrigatória.");
        }

        private static void ValidateUserId(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("Identificador do usuário inválido.");
        }

        private static void ValidateOccurrenceId(Guid occurrenceId)
        {
            if (occurrenceId == Guid.Empty)
                throw new ArgumentException("Identificador da ocorrência inválido.");
        }

        // Converte a entidade para o DTO de resposta
        private static WorkOccurrenceResponse MapToResponse(WorkOccurrence occurrence)
        {
            return new WorkOccurrenceResponse
            {
                WorkOccurrenceId = occurrence.WorkOccurrenceId,
                EmploymentId = occurrence.EmploymentId,
                Category = occurrence.Category,
                Description = occurrence.Description,
                OccurredAt = occurrence.OccurredAt,
                CreatedAt = occurrence.CreatedAt,
                UpdatedAt = occurrence.UpdatedAt,
                Version = occurrence.Version
            };
        }
    }
}

