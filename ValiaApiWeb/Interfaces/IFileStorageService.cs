
namespace ValiaApiWeb.Services.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveAsync(Stream content, CancellationToken cancellationToken = default);
        Task<Stream> OpenReadAsync(string storageKey, CancellationToken cancellationToken = default);
        Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default);
    }
}

