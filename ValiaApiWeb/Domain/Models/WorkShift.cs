namespace ValiaApiWeb.Domain.Models
{
    public class WorkShift
    {
        public Guid WorkShiftId { get; private set; } = Guid.NewGuid(); //Identificador da jornada
        public Guid EmploymentId { get; private set; } //Vínculo ao qual pertence
        public DateTimeOffset StartTime { get; private set; } //Data e horário de entrada
        public DateTimeOffset EndTime { get; private set; } //Data e horário de saída
        public DateTimeOffset? BreakStart { get; private set; } //Início do intervalo
        public DateTimeOffset? BreakEnd { get; private set; } //Fim do intervalo
        public string? Notes { get; private set; } //Observações do trabalhador
        public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow; //Data de criação
        public DateTimeOffset? UpdatedAt { get; private set; } //Última atualização
        public int Version { get; private set; } = 1; //Controle futuro de concorrência
        public Employment Employment { get; private set; } = null!; //Navegação para o vínculo

        protected WorkShift() { }

        public WorkShift(Guid employmentId, DateTimeOffset startTime, DateTimeOffset endTime, DateTimeOffset? breakStart, DateTimeOffset? breakEnd, string? notes)
        {
            if (employmentId == Guid.Empty)
                throw new ArgumentException("Vínculo profissional inválido.");

            ValidateTimes(startTime, endTime, breakStart, breakEnd);

            WorkShiftId = Guid.NewGuid();
            EmploymentId = employmentId;
            StartTime = startTime;
            EndTime = endTime;
            BreakStart = breakStart;
            BreakEnd = breakEnd;
            Notes = notes?.Trim();
            CreatedAt = DateTimeOffset.UtcNow;
            Version = 1;
        }

        public void Update(DateTimeOffset startTime, DateTimeOffset endTime, DateTimeOffset? breakStart, DateTimeOffset? breakEnd, string? notes)
        {
            ValidateTimes(startTime, endTime, breakStart, breakEnd);

            StartTime = startTime;
            EndTime = endTime;
            BreakStart = breakStart;
            BreakEnd = breakEnd;
            Notes = notes?.Trim();
            UpdatedAt = DateTimeOffset.UtcNow;
            Version++;
        }

        public TimeSpan GetWorkedDuration()
        {
            TimeSpan totalDuration = EndTime - StartTime;

            if (BreakStart.HasValue && BreakEnd.HasValue)
            {
                totalDuration -= BreakEnd.Value - BreakStart.Value;
            }

            return totalDuration;
        }

        private static void ValidateTimes(DateTimeOffset startTime, DateTimeOffset endTime, DateTimeOffset? breakStart, DateTimeOffset? breakEnd)
        {
            if (startTime == default || endTime == default)
                throw new ArgumentException("O início e o término da jornada são obrigatórios.");

            if (endTime <= startTime)
                throw new ArgumentException("O término deve ser posterior ao início da jornada.");

            if (breakStart.HasValue != breakEnd.HasValue)
                throw new ArgumentException("O início e o fim do intervalo devem ser informados juntos.");

            if (breakStart.HasValue && breakEnd.HasValue)
            {
                if (breakStart.Value < startTime || breakEnd.Value > endTime)
                {
                    throw new ArgumentException("O intervalo deve estar dentro da jornada.");
                }

                if (breakEnd.Value <= breakStart.Value)
                    throw new ArgumentException("O fim do intervalo deve ser posterior ao início.");
            }
        }

    }
}
