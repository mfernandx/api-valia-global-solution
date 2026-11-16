
using Microsoft.EntityFrameworkCore;
using ValiaApiWeb.Data;
using ValiaApiWeb.Domain.Exceptions;
using ValiaApiWeb.Domain.Models;
using ValiaApiWeb.DTOs.Requests;
using ValiaApiWeb.DTOs.Responses;
using ValiaApiWeb.Services.Interfaces;

namespace ValiaApiWeb.Services
{
    public class WorkShiftService : IWorkShiftService
    {
        private readonly ValiaDbContext _context;

        public WorkShiftService(ValiaDbContext context)
        {
            _context = context;
        }

        // Cadastra uma jornada de trabalho
        public async Task<WorkShiftResponse> CreateAsync(Guid userId, CreateWorkShiftRequest request)
        {
            ValidateUserId(userId);
            ArgumentNullException.ThrowIfNull(request);

            if (!request.EmploymentId.HasValue || request.EmploymentId.Value == Guid.Empty)
                throw new ArgumentException("O vínculo profissional é obrigatório.");

            if (!request.StartTime.HasValue || !request.EndTime.HasValue)
                throw new ArgumentException("Os horários de entrada e saída são obrigatórios.");

            Guid employmentId = request.EmploymentId.Value;

            // Verifica se o vínculo pertence ao usuário autenticado
            bool employmentExists = await _context.Employments.AnyAsync(e => e.EmploymentId == employmentId && e.UserId == userId && e.User.IsActive);

            if (!employmentExists)
                throw new KeyNotFoundException("Vínculo profissional não encontrado.");

            // A entidade valida os horários e o intervalo
            var workShift = new WorkShift(employmentId, request.StartTime.Value, request.EndTime.Value, request.BreakStart, request.BreakEnd, request.Notes);

            await ValidateOverlapAsync(employmentId, workShift.StartTime, workShift.EndTime);

            await _context.WorkShifts.AddAsync(workShift);
            await _context.SaveChangesAsync();

            return MapToResponse(workShift);
        }

        // Consulta uma jornada pertencente ao usuário
        public async Task<WorkShiftResponse?> GetByIdAsync(Guid userId, Guid workShiftId)
        {
            ValidateUserId(userId);
            ValidateWorkShiftId(workShiftId);

            WorkShift? workShift = await _context.WorkShifts.AsNoTracking().FirstOrDefaultAsync(w => w.WorkShiftId == workShiftId && w.Employment.UserId == userId);

            return workShift == null ? null : MapToResponse(workShift);
        }

        // Lista as jornadas de determinado vínculo profissional
        public async Task<IEnumerable<WorkShiftResponse>> GetByEmploymentIdAsync(Guid userId, Guid employmentId)
        {
            ValidateUserId(userId);

            if (employmentId == Guid.Empty)
                throw new ArgumentException("Identificador do vínculo profissional inválido.");

            bool employmentExists = await _context.Employments.AnyAsync(e => e.EmploymentId == employmentId && e.UserId == userId);

            if (!employmentExists)
                throw new KeyNotFoundException("Vínculo profissional não encontrado.");

            List<WorkShift> workShifts = await _context.WorkShifts.AsNoTracking().Where(w => w.EmploymentId == employmentId).OrderByDescending(w => w.StartTime).ToListAsync();

            return workShifts.Select(MapToResponse).ToList();
        }

        // Atualiza uma jornada existente
        public async Task<WorkShiftResponse> UpdateAsync(Guid userId, Guid workShiftId, UpdateWorkShiftRequest request)
        {
            ValidateUserId(userId);
            ValidateWorkShiftId(workShiftId);
            ArgumentNullException.ThrowIfNull(request);

            if (!request.StartTime.HasValue || !request.EndTime.HasValue)
                throw new ArgumentException("Os horários de entrada e saída são obrigatórios.");

            WorkShift workShift = await _context.WorkShifts.FirstOrDefaultAsync(w => w.WorkShiftId == workShiftId && w.Employment.UserId == userId)
                ?? throw new KeyNotFoundException("Jornada de trabalho não encontrada.");

            // Valida os novos horários antes de alterar a entidade
            var validatedShift = new WorkShift(workShift.EmploymentId, request.StartTime.Value, request.EndTime.Value, request.BreakStart, request.BreakEnd, request.Notes);

            await ValidateOverlapAsync(workShift.EmploymentId, validatedShift.StartTime, validatedShift.EndTime, workShift.WorkShiftId);

            workShift.Update(request.StartTime.Value, request.EndTime.Value, request.BreakStart, request.BreakEnd, request.Notes);

            await _context.SaveChangesAsync();

            return MapToResponse(workShift);
        }

        // Exclui uma jornada pertencente ao usuário
        public async Task DeleteAsync(Guid userId, Guid workShiftId)
        {
            ValidateUserId(userId);
            ValidateWorkShiftId(workShiftId);

            WorkShift workShift = await _context.WorkShifts.FirstOrDefaultAsync(w => w.WorkShiftId == workShiftId && w.Employment.UserId == userId)
                ?? throw new KeyNotFoundException("Jornada de trabalho não encontrada.");

            _context.WorkShifts.Remove(workShift);
            await _context.SaveChangesAsync();
        }

        // Impede jornadas com períodos sobrepostos no mesmo vínculo
        private async Task ValidateOverlapAsync(Guid employmentId, DateTimeOffset startTime, DateTimeOffset endTime, Guid? ignoredWorkShiftId = null)
        {
            bool overlaps = await _context.WorkShifts.AnyAsync(w =>
                w.EmploymentId == employmentId &&
                (!ignoredWorkShiftId.HasValue || w.WorkShiftId != ignoredWorkShiftId.Value) &&
                w.StartTime < endTime && w.EndTime > startTime);

            if (overlaps)
                throw new BusinessConflictException("Já existe uma jornada registrada que se sobrepõe ao período informado.");
        }

        // Valida o identificador do usuário
        private static void ValidateUserId(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("Identificador do usuário inválido.");
        }

        // Valida o identificador da jornada
        private static void ValidateWorkShiftId(Guid workShiftId)
        {
            if (workShiftId == Guid.Empty)
                throw new ArgumentException("Identificador da jornada inválido.");
        }

        // Converte a entidade para o DTO de resposta
        private static WorkShiftResponse MapToResponse(WorkShift workShift)
        {
            return new WorkShiftResponse
            {
                WorkShiftId = workShift.WorkShiftId,
                EmploymentId = workShift.EmploymentId,
                StartTime = workShift.StartTime,
                EndTime = workShift.EndTime,
                BreakStart = workShift.BreakStart,
                BreakEnd = workShift.BreakEnd,
                Notes = workShift.Notes,
                WorkedHours = (decimal)workShift.GetWorkedDuration().TotalHours,
                Version = workShift.Version,
                CreatedAt = workShift.CreatedAt,
                UpdatedAt = workShift.UpdatedAt
            };
        }
    }
}

