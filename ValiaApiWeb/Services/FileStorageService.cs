using ValiaApiWeb.Services.Interfaces;

namespace ValiaApiWeb.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _storagePath;

        public FileStorageService(IConfiguration configuration, IWebHostEnvironment environment)
        {
            string configuredPath = configuration["FileStorage:Path"] ?? "PrivateStorage";

            _storagePath = Path.IsPathRooted(configuredPath)
                ? Path.GetFullPath(configuredPath)
                : Path.GetFullPath(Path.Combine(environment.ContentRootPath, configuredPath));

            string publicRoot = Path.GetFullPath(environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot"));
            string publicRootWithSeparator = Path.TrimEndingDirectorySeparator(publicRoot) + Path.DirectorySeparatorChar;

            if (_storagePath.Equals(publicRoot, StringComparison.OrdinalIgnoreCase) ||
                _storagePath.StartsWith(publicRootWithSeparator, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("O armazenamento privado não pode ficar dentro de wwwroot.");

            Directory.CreateDirectory(_storagePath);
        }

        // Salva o arquivo utilizando um identificador aleatório
        public async Task<string> SaveAsync(Stream content, CancellationToken cancellationToken = default)
        {
            string storageKey = Guid.NewGuid().ToString("N");
            string filePath = GetFilePath(storageKey);

            await using var fileStream = new FileStream(
                filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, useAsync: true);

            await content.CopyToAsync(fileStream, cancellationToken);
            return storageKey;
        }

        // Abre o arquivo para leitura
        public Task<Stream> OpenReadAsync(string storageKey, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string filePath = GetFilePath(storageKey);

            if (!File.Exists(filePath))
                throw new FileNotFoundException("O arquivo solicitado não foi encontrado.");

            Stream stream = new FileStream(
                filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, useAsync: true);

            return Task.FromResult(stream);
        }

        // Exclui o arquivo armazenado
        public Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string filePath = GetFilePath(storageKey);

            if (File.Exists(filePath))
                File.Delete(filePath);

            return Task.CompletedTask;
        }

        // Impede que o identificador seja utilizado para acessar outros caminhos
        private string GetFilePath(string storageKey)
        {
            if (storageKey.Length != 32 || !Guid.TryParseExact(storageKey, "N", out _))
                throw new ArgumentException("Identificador de armazenamento inválido.");

            return Path.Combine(_storagePath, storageKey);
        }
    }
}

