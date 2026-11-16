using ValiaApiWeb.Domain.Enums;

namespace ValiaApiWeb.Domain.Models
{
    public class Employment
    {
        public Guid EmploymentId { get; private set; } // Identificador único do vínculo profissional
        public Guid UserId { get; private set; } // Identificador do usuário proprietário
        public string JobTitle { get; private set; } = string.Empty; // Cargo ou função exercida
        public EnumEmploymentType EmploymentType { get; private set; } // Tipo de vínculo profissional
        public DateOnly AdmissionDate { get; private set; } // Data de admissão
        public DateOnly? TerminationDate { get; private set; } // Data de desligamento, quando aplicável
        public decimal? BaseSalary { get; private set; } // Salário-base informado pelo trabalhador
        public decimal? WeeklyWorkload { get; private set; } // Carga horária semanal prevista
        public string? ProfessionalCategory { get; private set; } // Categoria profissional
        public DateTimeOffset CreatedAt { get; private set; } // Data de criação do registro
        public DateTimeOffset? UpdatedAt { get; private set; } // Data da última atualização
        public int Version { get; private set; } // Versão para controle de concorrência
        public User User { get; private set; } = null!; // Usuário proprietário do vínculo
        public ICollection<WorkShift> WorkShifts { get; private set; } = new List<WorkShift>(); // Jornadas associadas ao vínculo
        public ICollection<WorkOccurrence> WorkOccurrences { get; private set; } = new List<WorkOccurrence>(); // Ocorrências associadas ao vínculo
        public ICollection<WorkConditionAssessment> WorkConditionAssessments { get; private set; } = new List<WorkConditionAssessment>(); // Avaliações das condições de trabalho do vínculo
        public ICollection<Evidence> Evidences { get; private set; } = new List<Evidence>(); // Documentos associados ao vínculo profissional

        protected Employment() { }

        public Employment(Guid userId, string jobTitle, EnumEmploymentType employmentType, DateOnly admissionDate, DateOnly? terminationDate, decimal? baseSalary, decimal? weeklyWorkload, string? professionalCategory)
        {
            Validate(userId, jobTitle, employmentType, admissionDate, terminationDate, baseSalary, weeklyWorkload);

            EmploymentId = Guid.NewGuid();
            UserId = userId;
            JobTitle = jobTitle.Trim();
            EmploymentType = employmentType;
            AdmissionDate = admissionDate;
            TerminationDate = terminationDate;
            BaseSalary = baseSalary;
            WeeklyWorkload = weeklyWorkload;
            ProfessionalCategory = professionalCategory?.Trim();
            CreatedAt = DateTimeOffset.UtcNow;
            Version = 1;
        }

        public void Update(string jobTitle, EnumEmploymentType employmentType, DateOnly admissionDate, DateOnly? terminationDate, decimal? baseSalary, decimal? weeklyWorkload, string? professionalCategory)
        {
            Validate(UserId, jobTitle, employmentType, admissionDate, terminationDate, baseSalary, weeklyWorkload);

            JobTitle = jobTitle.Trim();
            EmploymentType = employmentType;
            AdmissionDate = admissionDate;
            TerminationDate = terminationDate;
            BaseSalary = baseSalary;
            WeeklyWorkload = weeklyWorkload;
            ProfessionalCategory = professionalCategory?.Trim();
            UpdatedAt = DateTimeOffset.UtcNow;
            Version++;
        }

        private static void Validate(Guid userId, string jobTitle, EnumEmploymentType employmentType, DateOnly admissionDate, DateOnly? terminationDate, decimal? baseSalary, decimal? weeklyWorkload)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("Usuário inválido.");

            if (string.IsNullOrWhiteSpace(jobTitle))
                throw new ArgumentException("O cargo é obrigatório.");

            if (!Enum.IsDefined(employmentType))
                throw new ArgumentException("Tipo de vínculo inválido.");

            if (admissionDate == default)
                throw new ArgumentException("A data de admissão é obrigatória.");

            if (terminationDate.HasValue && terminationDate.Value < admissionDate)
                throw new ArgumentException("A data de desligamento não pode ser anterior à admissão.");

            if (baseSalary.HasValue && baseSalary.Value < 0)
                throw new ArgumentException("O salário não pode ser negativo.");

            if (weeklyWorkload.HasValue && weeklyWorkload.Value < 0)
                throw new ArgumentException("A carga horária semanal não pode ser negativa.");
        }

    }
}
