
namespace ValiaApiWeb.DTOs.Responses
{
    public class EvidenceFileResponse
    {
        public Stream Content { get; set; } = Stream.Null; // Fluxo de leitura do arquivo
        public string ContentType { get; set; } = string.Empty; // Tipo MIME
        public string FileName { get; set; } = string.Empty; // Nome para download
    }
}

