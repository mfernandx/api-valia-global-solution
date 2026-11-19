namespace ValiaApiWeb.DTOs.Responses
{
    public class WorkShiftResponse
    {
        public Guid WorkShiftId { get; set; } // Identificador da jornada
        public Guid EmploymentId { get; set; } // Vínculo profissional associado
        public DateTimeOffset StartTime { get; set; } // Entrada
        public DateTimeOffset EndTime { get; set; } // Saída
        public DateTimeOffset? BreakStart { get; set; } // Início do intervalo
        public DateTimeOffset? BreakEnd { get; set; } // Fim do intervalo
        public string? Notes { get; set; } // Observações
        public decimal WorkedHours { get; set; } // Total de horas efetivamente trabalhadas
        public int Version { get; set; } // Versão do registro
        public DateTimeOffset CreatedAt { get; set; } // Data de criação
        public DateTimeOffset? UpdatedAt { get; set; } // Última atualização
    }
}

