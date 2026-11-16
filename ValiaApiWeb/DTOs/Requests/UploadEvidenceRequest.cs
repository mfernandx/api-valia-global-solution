using System.ComponentModel.DataAnnotations;
using ValiaApiWeb.Domain.Enums;

namespace ValiaApiWeb.DTOs.Requests
{
    public class UploadEvidenceRequest
    {
        [Required(ErrorMessage = "O título é obrigatório.")]
        [MaxLength(150)]
        public string Title { get; set; } = string.Empty; // Título do documento

        [Required(ErrorMessage = "A categoria é obrigatória.")]
        public EnumEvidenceCategory? Category { get; set; } // Categoria do documento

        public Guid? EmploymentId { get; set; } // Vínculo profissional opcional

        [Required(ErrorMessage = "O arquivo é obrigatório.")]
        public IFormFile? File { get; set; } // Arquivo enviado pelo usuário
    }
}

