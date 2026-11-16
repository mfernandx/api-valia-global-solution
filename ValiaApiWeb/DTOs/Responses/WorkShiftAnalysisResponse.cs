
namespace ValiaApiWeb.DTOs.Responses
{
    public class WorkShiftAnalysisResponse
    {
        public Guid EmploymentId { get; set; } // Vínculo profissional analisado
        public DateOnly StartDate { get; set; } // Data inicial do período
        public DateOnly EndDate { get; set; } // Data final do período
        public int TotalShifts { get; set; } // Quantidade de jornadas
        public decimal TotalWorkedHours { get; set; } // Total de horas trabalhadas
        public decimal AverageWorkedHours { get; set; } // Média de horas por jornada
        public int ShiftsOverEightHours { get; set; } // Jornadas acima de oito horas
        public int RestPeriodsBelowElevenHours { get; set; } // Descansos inferiores a onze horas
        public List<WorkShiftRestResponse> RestPeriods { get; set; } = new(); // Intervalos entre jornadas
    }
}

