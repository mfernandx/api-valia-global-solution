using System.Text.Json.Serialization;

namespace ValiaApiWeb.Domain.Models
{
    public class User
    {
        public Guid UserId { get; private set; } //Identificador do usuário.
        public string FullName { get; private set; } = string.Empty; //Nome completo
        public string Email { get; private set; } = string.Empty; //Email
        [JsonIgnore]
        public string PasswordHash { get; private set; } = string.Empty; //Hash da senha
        public string? PhoneNumber { get; private set; } = string.Empty; //Número de telefone
        public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow; //Data do registro no sistema
        public DateTimeOffset? UpdatedAt { get; private set; } //Última atualização
        public bool IsActive { get; private set; } //Indica se a conta está ativa
        public ICollection<Employment> Employments { get; private set; } = new List<Employment>(); //Coleção de vínculos associados ao usuário.
        public ICollection<Evidence> Evidences { get; private set; } = new List<Evidence>(); // Documentos armazenados pelo usuário
        public ICollection<CommunityPost> CommunityPosts { get; private set; } = new List<CommunityPost>(); // Publicações criadas pelo usuário
        public ICollection<CommunityComment> CommunityComments { get; private set; } = new List<CommunityComment>(); // Comentários criados pelo usuário

        protected User() { }

        public User(string fullName, string email, string passwordHash, string? phoneNumber = null)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("O nome completo é obrigatório.");

            ValidateEmail(email);

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("O hash da senha é obrigatório.");

            UserId = Guid.NewGuid();
            FullName = fullName.Trim();
            Email = email.Trim().ToLowerInvariant();
            PasswordHash = passwordHash;
            PhoneNumber = NormalizePhoneNumber(phoneNumber);
            IsActive = true;
            CreatedAt = DateTimeOffset.UtcNow;
        }

        public void UpdateEmail(string email)
        {
            ValidateEmail(email);

            Email = email.Trim().ToLowerInvariant();
            UpdatedAt = DateTimeOffset.UtcNow;
        }

        public void UpdatePhoneNumber(string phoneNumber)
        {
            PhoneNumber = NormalizePhoneNumber(phoneNumber);
            UpdatedAt = DateTimeOffset.UtcNow;
        }

        public void UpdatePasswordHash(string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("O hash da senha é obrigatório.");

            PasswordHash = passwordHash;
            UpdatedAt = DateTimeOffset.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTimeOffset.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTimeOffset.UtcNow;
        }

        private static void ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("O e-mail é obrigatório.");
        }

        private static string? NormalizePhoneNumber(string? phoneNumber)
        {
            return string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber.Trim();
        }

    }
}
