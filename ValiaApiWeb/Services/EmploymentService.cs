
using Microsoft.EntityFrameworkCore;
using ValiaApiWeb.Data;
using ValiaApiWeb.Domain.Exceptions;
using ValiaApiWeb.Domain.Models;
using ValiaApiWeb.DTOs.Requests;
using ValiaApiWeb.DTOs.Responses;
using ValiaApiWeb.Services.Interfaces;

namespace ValiaApiWeb.Services
{
    public class EmploymentService : IEmploymentService
    {
        private readonly ValiaDbContext _context;

        public EmploymentService(ValiaDbContext context)
        {
            _context = context;
        }

        // Cadastra um vínculo profissional para o usuário
        public async Task<EmploymentResponse> CreateAsync(Guid userId, CreateEmploymentRequest request)
        {
            ValidateUserId(userId);

            ArgumentNullException.ThrowIfNull(request);

            ValidateEmploymentData(request.EmploymentType, request.AdmissionDate);

            // Confirma que o usuário existe e está ativo
            bool activeUserExists = await _context.Users.AnyAsync(u => u.UserId == userId && u.IsActive);

            if (!activeUserExists)
                throw new KeyNotFoundException("Usuário ativo não encontrado.");

            // Cria a entidade utilizando seu construtor
            var employment = new Employment(
                userId,
                request.JobTitle,
                request.EmploymentType!.Value,
                request.AdmissionDate!.Value,
                request.TerminationDate,
                request.BaseSalary,
                request.WeeklyWorkload,
                request.ProfessionalCategory
            );

            await _context.Employments.AddAsync(employment);

            await _context.SaveChangesAsync();

            return MapToResponse(employment);
        }

        // Consulta um vínculo pertencente ao usuário
        public async Task<EmploymentResponse?> GetByIdAsync(Guid userId, Guid employmentId)
        {
            ValidateUserId(userId);
            ValidateEmploymentId(employmentId);

            Employment? employment = await _context.Employments.AsNoTracking().FirstOrDefaultAsync(e => e.EmploymentId == employmentId && e.UserId == userId);

            return employment == null ? null : MapToResponse(employment);
        }

        // Lista todos os vínculos profissionais do usuário
        public async Task<IEnumerable<EmploymentResponse>> GetByUserIdAsync(Guid userId)
        {
            ValidateUserId(userId);

            List<Employment> employments = await _context.Employments.AsNoTracking().Where(e => e.UserId == userId).OrderByDescending(e => e.AdmissionDate).ToListAsync();

            return employments.Select(MapToResponse).ToList();
        }

        // Atualiza um vínculo profissional existente
        public async Task<EmploymentResponse> UpdateAsync(Guid userId, Guid employmentId, UpdateEmploymentRequest request)
        {
            ValidateUserId(userId);
            ValidateEmploymentId(employmentId);

            ArgumentNullException.ThrowIfNull(request);

            ValidateEmploymentData(request.EmploymentType, request.AdmissionDate);

            // Consulta somente o vínculo do usuário autenticado
            Employment employment = await _context.Employments.FirstOrDefaultAsync(e => e.EmploymentId == employmentId && e.UserId == userId)
                ?? throw new KeyNotFoundException("Vínculo profissional não encontrado.");

            // Atualiza por meio do método da entidade
            employment.Update(
                request.JobTitle,
                request.EmploymentType!.Value,
                request.AdmissionDate!.Value,
                request.TerminationDate,
                request.BaseSalary,
                request.WeeklyWorkload,
                request.ProfessionalCategory
            );

            await _context.SaveChangesAsync();

            return MapToResponse(employment);
        }

        // Exclui um vínculo que não possui registros associados
        public async Task DeleteAsync(Guid userId, Guid employmentId)
        {
            ValidateUserId(userId);
            ValidateEmploymentId(employmentId);

            Employment employment = await _context.Employments.FirstOrDefaultAsync(e => e.EmploymentId == employmentId && e.UserId == userId)
                ?? throw new KeyNotFoundException("Vínculo profissional não encontrado.");

            // Verifica se existem jornadas associadas
            bool hasWorkShifts = await _context.WorkShifts.AnyAsync(w => w.EmploymentId == employmentId);

            // Verifica se existem ocorrências associadas
            bool hasOccurrences = await _context.WorkOccurrences.AnyAsync(o => o.EmploymentId == employmentId);

            // Verifica se existem questionários associados
            bool hasAssessments = await _context.WorkConditionAssessments.AnyAsync(a => a.EmploymentId == employmentId);

            // Verifica se existem documentos associados
            bool hasEvidences = await _context.Evidences.AnyAsync(e => e.EmploymentId == employmentId);

            if (hasWorkShifts || hasOccurrences || hasAssessments || hasEvidences)
            {
                throw new BusinessConflictException("Não é possível excluir este vínculo porque existem registros associados.");
            }

            _context.Employments.Remove(employment);

            await _context.SaveChangesAsync();
        }

        // Valida o identificador do usuário
        private static void ValidateUserId(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("Identificador do usuário inválido.");
        }

        // Valida o identificador do vínculo
        private static void ValidateEmploymentId(Guid employmentId)
        {
            if (employmentId == Guid.Empty)
                throw new ArgumentException("Identificador do vínculo profissional inválido.");
        }

        // Valida os campos obrigatórios do vínculo
        private static void ValidateEmploymentData(Domain.Enums.EnumEmploymentType? employmentType, DateOnly? admissionDate)
        {
            if (!employmentType.HasValue || !Enum.IsDefined(employmentType.Value))
            {
                throw new ArgumentException("O tipo de vínculo profissional é inválido.");
            }

            if (!admissionDate.HasValue)
                throw new ArgumentException("A data de admissão é obrigatória.");
        }

        // Converte a entidade para o DTO de resposta
        private static EmploymentResponse MapToResponse(Employment employment)
        {
            return new EmploymentResponse
            {
                EmploymentId = employment.EmploymentId,
                UserId = employment.UserId,
                JobTitle = employment.JobTitle,
                EmploymentType = employment.EmploymentType,
                AdmissionDate = employment.AdmissionDate,
                TerminationDate = employment.TerminationDate,
                BaseSalary = employment.BaseSalary,
                WeeklyWorkload = employment.WeeklyWorkload,
                ProfessionalCategory = employment.ProfessionalCategory,
                Version = employment.Version,
                CreatedAt = employment.CreatedAt,
                UpdatedAt = employment.UpdatedAt
            };
        }
    }
}

