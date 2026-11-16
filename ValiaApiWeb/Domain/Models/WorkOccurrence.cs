using ValiaApiWeb.Domain.Enums;

namespace ValiaApiWeb.Domain.Models
{
    public class WorkOccurrence
    {
        public Guid WorkOccurrenceId { get; private set; } = Guid.NewGuid(); //Identificador da ocorrência
        public Guid EmploymentId { get; private set; } // Vínculo profissional associado
        public OccurrenceCategory Category { get; private set; } //Categoria do acontecimento
        public string Description { get; private set; } = string.Empty; //Descrição informada pelo trabalhador
        public DateTimeOffset OccurredAt { get; private set; } //Data e horário do acontecimento
        public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow; //Data do registro no sistema
        public DateTimeOffset? UpdatedAt { get; private set; } //Última atualização
        public int Version { get; private set; } = 1; //Controle futuro de concorrência
        public Employment Employment { get; private set; } = null!; //Propriedade de navegação

        protected WorkOccurrence() { }

        public WorkOccurrence(Guid employmentId, OccurrenceCategory category, string description, DateTimeOffset occurredAt)
        {
            Validate(category, description);

            if (employmentId == Guid.Empty)
                throw new ArgumentException("Vínculo profissional inválido.");

            if (occurredAt == default)
                throw new ArgumentException("A data da ocorrência é obrigatória.");

            WorkOccurrenceId = Guid.NewGuid();
            EmploymentId = employmentId;
            Category = category;
            Description = description.Trim();
            OccurredAt = occurredAt;
            CreatedAt = DateTimeOffset.UtcNow;
            Version = 1;
        }

        public void Update(OccurrenceCategory category, string description, DateTimeOffset occurredAt)
        {
            Validate(category, description);

            if (occurredAt == default)
                throw new ArgumentException("A data da ocorrência é obrigatória.");

            Category = category;
            Description = description.Trim();
            OccurredAt = occurredAt;
            UpdatedAt = DateTimeOffset.UtcNow;
            Version++;
        }

        private static void Validate(OccurrenceCategory category, string description)
        {
            if (!Enum.IsDefined(category))
                throw new ArgumentException("Categoria de ocorrência inválida.");

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("A descrição é obrigatória.");
        }
    }
}
