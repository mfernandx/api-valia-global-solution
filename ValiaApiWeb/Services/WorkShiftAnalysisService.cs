using Microsoft.EntityFrameworkCore;
using ValiaApiWeb.Data;
using ValiaApiWeb.DTOs.Responses;
using ValiaApiWeb.Services.Interfaces;

namespace ValiaApiWeb.Services
{
    public class WorkShiftAnalysisService : IWorkShiftAnalysisService
    {
        private readonly ValiaDbContext _context;

        public WorkShiftAnalysisService(ValiaDbContext context)
        {
            _context = context;
        }

        // Analisa as jornadas registradas em um período
        public async Task<WorkShiftAnalysisResponse> AnalyzeAsync(Guid userId, Guid employmentId, DateOnly startDate, DateOnly endDate)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("Identificador do usuário inválido.");

            if (employmentId == Guid.Empty)
                throw new ArgumentException("Identificador do vínculo profissional inválido.");

            if (startDate > endDate)
                throw new ArgumentException("A data inicial não pode ser posterior à data final.");

            // Limita o período para evitar consultas excessivamente grandes
            if (endDate.DayNumber - startDate.DayNumber > 366)
                throw new ArgumentException("O período de análise não pode ultrapassar 366 dias.");

            // Verifica se o vínculo pertence a um usuário ativo
            bool employmentExists = await _context.Employments.AnyAsync(e => e.EmploymentId == employmentId && e.UserId == userId && e.User.IsActive);

            if (!employmentExists)
                throw new KeyNotFoundException("Vínculo profissional não encontrado.");

            // Consulta as jornadas do vínculo, ordenadas cronologicamente
            var allShifts = await _context.WorkShifts
                .AsNoTracking()
                .Where(w => w.EmploymentId == employmentId)
                .OrderBy(w => w.StartTime)
                .ToListAsync();

            // Filtra pela data local de início da jornada
            var periodShifts = allShifts
                .Where(w =>
                {
                    DateOnly shiftDate = DateOnly.FromDateTime(w.StartTime.Date);
                    return shiftDate >= startDate && shiftDate <= endDate;
                })
                .ToList();

            decimal totalWorkedHours = periodShifts.Sum(w => (decimal)w.GetWorkedDuration().TotalHours);
            int totalShifts = periodShifts.Count;
            decimal averageWorkedHours = totalShifts > 0 ? totalWorkedHours / totalShifts : 0;
            int shiftsOverEightHours = periodShifts.Count(w => w.GetWorkedDuration().TotalHours > 8);

            // Calcula os descansos anteriores às jornadas incluídas no período
            var restPeriods = new List<WorkShiftRestResponse>();

            for (int i = 0; i < allShifts.Count - 1; i++)
            {
                var previous = allShifts[i];
                var next = allShifts[i + 1];

                DateOnly nextShiftDate = DateOnly.FromDateTime(next.StartTime.Date);

                if (nextShiftDate < startDate || nextShiftDate > endDate)
                    continue;

                decimal restHours = (decimal)(next.StartTime - previous.EndTime).TotalHours;

                restPeriods.Add(new WorkShiftRestResponse
                {
                    PreviousWorkShiftId = previous.WorkShiftId,
                    NextWorkShiftId = next.WorkShiftId,
                    PreviousEndTime = previous.EndTime,
                    NextStartTime = next.StartTime,
                    RestHours = restHours,
                    BelowElevenHours = restHours < 11
                });
            }

            return new WorkShiftAnalysisResponse
            {
                EmploymentId = employmentId,
                StartDate = startDate,
                EndDate = endDate,
                TotalShifts = totalShifts,
                TotalWorkedHours = Math.Round(totalWorkedHours, 2),
                AverageWorkedHours = Math.Round(averageWorkedHours, 2),
                ShiftsOverEightHours = shiftsOverEightHours,
                RestPeriodsBelowElevenHours = restPeriods.Count(r => r.BelowElevenHours),
                RestPeriods = restPeriods
            };
        }
    }
}

