
using System.ComponentModel.DataAnnotations;
using ValiaApiWeb.Domain.Enums;

namespace ValiaApiWeb.DTOs.Requests
{
    public class CreateEmploymentRequest
    {
        [Required(ErrorMessage = "O cargo é obrigatório.")]
        [MaxLength(150)]
        public string JobTitle { get; set; } = string.Empty; // Cargo ou função exercida

        [Required(ErrorMessage = "O tipo de vínculo é obrigatório.")]
        public EnumEmploymentType? EmploymentType { get; set; } // Tipo de contratação

        [Required(ErrorMessage = "A data de admissão é obrigatória.")]
        public DateOnly? AdmissionDate { get; set; } // Data de início do vínculo

        public DateOnly? TerminationDate { get; set; } // Data de encerramento, se houver

        [Range(0.01, 999999999.99)]
        public decimal? BaseSalary { get; set; } // Salário-base informado

        [Range(0.01, 168)]
        public decimal? WeeklyWorkload { get; set; } // Jornada semanal em horas

        [MaxLength(150)]
        public string? ProfessionalCategory { get; set; } // Categoria profissional
    }
}

