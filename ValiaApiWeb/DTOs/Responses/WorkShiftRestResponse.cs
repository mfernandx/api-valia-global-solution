
namespace ValiaApiWeb.DTOs.Responses
{
    public class WorkShiftRestResponse
    {
        public Guid PreviousWorkShiftId { get; set; } // Jornada anterior
        public Guid NextWorkShiftId { get; set; } // Jornada seguinte
        public DateTimeOffset PreviousEndTime { get; set; } // Saída anterior
        public DateTimeOffset NextStartTime { get; set; } // Próxima entrada
        public decimal RestHours { get; set; } // Horas de descanso
        public bool BelowElevenHours { get; set; } // Indica descanso inferior a 11h
    }
}

