namespace ValiaApiWeb.Domain.Models
{
    public class IdempotencyRecord
    {
        public Guid IdempotencyRecordId { get; private set; } = Guid.NewGuid();
        public Guid UserId { get; private set; }
        public string Operation { get; private set; } = string.Empty;
        public string IdempotencyKey { get; private set; } = string.Empty;
        public string RequestHash { get; private set; } = string.Empty;
        public string ResponseBody { get; private set; } = string.Empty;
        public int StatusCode { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;

        protected IdempotencyRecord() { }

        public IdempotencyRecord(Guid userId, string operation, string idempotencyKey,
            string requestHash, string responseBody, int statusCode)
        {
            if (userId == Guid.Empty) throw new ArgumentException("Usuário inválido.");
            if (string.IsNullOrWhiteSpace(operation)) throw new ArgumentException("Operação obrigatória.");
            if (string.IsNullOrWhiteSpace(idempotencyKey)) throw new ArgumentException("Chave obrigatória.");
            if (string.IsNullOrWhiteSpace(requestHash)) throw new ArgumentException("Hash obrigatório.");

            UserId = userId;
            Operation = operation;
            IdempotencyKey = idempotencyKey;
            RequestHash = requestHash;
            ResponseBody = responseBody ?? string.Empty;
            StatusCode = statusCode;
            CreatedAt = DateTimeOffset.UtcNow;
        }
    }
}
