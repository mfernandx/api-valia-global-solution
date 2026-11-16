using Microsoft.EntityFrameworkCore;
using ValiaApiWeb.Data;
using ValiaApiWeb.DTOs.Requests;
using ValiaApiWeb.DTOs.Responses;
using ValiaApiWeb.Services.Interfaces;
using ValiaApiWeb.Domain.Models;

namespace ValiaApi.Services
{
    public class EvidenceService : IEvidenceService
    {
        private const long MaxFileSize = 10 * 1024 * 1024;
        private readonly ValiaDbContext _context;
        private readonly IFileStorageService _fileStorage;

        public EvidenceService(ValiaDbContext context, IFileStorageService fileStorage)
        {
            _context = context;
            _fileStorage = fileStorage;
        }

        // Envia um documento e registra seus metadados
        public async Task<EvidenceResponse> UploadAsync(Guid userId, UploadEvidenceRequest request)
        {
            ValidateUserId(userId);
            ArgumentNullException.ThrowIfNull(request);

            if (string.IsNullOrWhiteSpace(request.Title) || request.Title.Length > 150)
                throw new ArgumentException("O título deve conter entre 1 e 150 caracteres.");

            if (!request.Category.HasValue || !Enum.IsDefined(request.Category.Value))
                throw new ArgumentException("A categoria do documento é inválida.");

            if (request.File == null || request.File.Length == 0 || request.File.Length > MaxFileSize)
                throw new ArgumentException("O arquivo deve possuir entre 1 byte e 10 MB.");

            if (request.EmploymentId == Guid.Empty)
                throw new ArgumentException("Identificador do vínculo profissional inválido.");

            bool activeUserExists = await _context.Users.AnyAsync(u => u.UserId == userId && u.IsActive);

            if (!activeUserExists)
                throw new KeyNotFoundException("Usuário ativo não encontrado.");

            if (request.EmploymentId.HasValue)
            {
                bool employmentExists = await _context.Employments
                    .AnyAsync(e => e.EmploymentId == request.EmploymentId.Value && e.UserId == userId);

                if (!employmentExists)
                    throw new KeyNotFoundException("Vínculo profissional não encontrado.");
            }

            string extension = Path.GetExtension(request.File.FileName).ToLowerInvariant();
            string contentType = GetValidatedContentType(extension, request.File.ContentType);
            string originalFileName = Path.GetFileName(request.File.FileName);

            await using var fileStream = request.File.OpenReadStream();
            await ValidateFileSignatureAsync(fileStream, extension);

            string storageKey = await _fileStorage.SaveAsync(fileStream);

            try
            {
                var evidence = new Evidence(
                    userId, request.EmploymentId, request.Title, request.Category.Value,
                    originalFileName, storageKey, contentType, request.File.Length);

                await _context.Evidences.AddAsync(evidence);
                await _context.SaveChangesAsync();

                return MapToResponse(evidence);
            }
            catch
            {
                await _fileStorage.DeleteAsync(storageKey);
                throw;
            }
        }

        // Consulta os metadados de um documento
        public async Task<EvidenceResponse?> GetByIdAsync(Guid userId, Guid evidenceId)
        {
            ValidateIds(userId, evidenceId);

            Evidence? evidence = await _context.Evidences.AsNoTracking()
                .FirstOrDefaultAsync(e => e.EvidenceId == evidenceId && e.UserId == userId);

            return evidence == null ? null : MapToResponse(evidence);
        }

        // Lista os documentos do usuário
        public async Task<IEnumerable<EvidenceResponse>> GetByUserIdAsync(Guid userId)
        {
            ValidateUserId(userId);

            List<Evidence> evidences = await _context.Evidences.AsNoTracking()
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

            return evidences.Select(MapToResponse).ToList();
        }

        // Abre um documento após verificar sua propriedade
        public async Task<EvidenceFileResponse> DownloadAsync(Guid userId, Guid evidenceId)
        {
            ValidateIds(userId, evidenceId);

            Evidence evidence = await _context.Evidences.AsNoTracking()
                .FirstOrDefaultAsync(e => e.EvidenceId == evidenceId && e.UserId == userId)
                ?? throw new KeyNotFoundException("Documento não encontrado.");

            Stream content = await _fileStorage.OpenReadAsync(evidence.StorageKey);

            return new EvidenceFileResponse
            {
                Content = content,
                ContentType = evidence.ContentType,
                FileName = evidence.FileName
            };
        }

        // Exclui o documento e seu arquivo armazenado
        public async Task DeleteAsync(Guid userId, Guid evidenceId)
        {
            ValidateIds(userId, evidenceId);

            Evidence evidence = await _context.Evidences
                .FirstOrDefaultAsync(e => e.EvidenceId == evidenceId && e.UserId == userId)
                ?? throw new KeyNotFoundException("Documento não encontrado.");

            // Remove os metadados antes do arquivo físico
            _context.Evidences.Remove(evidence);
            await _context.SaveChangesAsync();

            await _fileStorage.DeleteAsync(evidence.StorageKey);
        }

        // Valida a combinação entre extensão e tipo MIME
        private static string GetValidatedContentType(string extension, string suppliedContentType)
        {
            var allowedTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                [".pdf"] = "application/pdf",
                [".jpg"] = "image/jpeg",
                [".jpeg"] = "image/jpeg",
                [".png"] = "image/png"
            };

            if (!allowedTypes.TryGetValue(extension, out string? expectedType) ||
                !string.Equals(suppliedContentType, expectedType, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Formato de arquivo não permitido. Utilize PDF, JPG ou PNG.");

            return expectedType;
        }

        // Verifica a assinatura inicial do arquivo
        private static async Task ValidateFileSignatureAsync(Stream stream, string extension)
        {
            byte[] header = new byte[8];
            int bytesRead = await stream.ReadAsync(header.AsMemory(0, header.Length));

            bool valid = extension switch
            {
                ".pdf" => bytesRead >= 5 && header.AsSpan(0, 5).SequenceEqual("%PDF-"u8),
                ".jpg" or ".jpeg" => bytesRead >= 3 && header.AsSpan(0, 3).SequenceEqual(new byte[] { 0xFF, 0xD8, 0xFF }),
                ".png" => bytesRead >= 8 && header.AsSpan(0, 8).SequenceEqual(
                    new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }),
                _ => false
            };

            if (!valid)
                throw new ArgumentException("O conteúdo do arquivo não corresponde ao formato informado.");

            if (!stream.CanSeek)
                throw new ArgumentException("Não foi possível validar o arquivo enviado.");

            stream.Position = 0;
        }

        private static void ValidateUserId(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("Identificador do usuário inválido.");
        }

        private static void ValidateIds(Guid userId, Guid evidenceId)
        {
            ValidateUserId(userId);

            if (evidenceId == Guid.Empty)
                throw new ArgumentException("Identificador do documento inválido.");
        }

        // Converte a entidade para o DTO de resposta
        private static EvidenceResponse MapToResponse(Evidence evidence)
        {
            return new EvidenceResponse
            {
                EvidenceId = evidence.EvidenceId,
                UserId = evidence.UserId,
                EmploymentId = evidence.EmploymentId,
                Title = evidence.Title,
                Category = evidence.Category,
                FileName = evidence.FileName,
                ContentType = evidence.ContentType,
                FileSize = evidence.FileSize,
                CreatedAt = evidence.CreatedAt
            };
        }
    }
}
