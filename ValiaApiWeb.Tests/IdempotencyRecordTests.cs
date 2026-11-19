using System;
using ValiaApiWeb.Domain.Models;
using Xunit;

namespace ValiaApiWeb.Tests
{
    public class IdempotencyRecordTests
    {
        [Fact]
        public void Construtor_ComUsuarioVazio_DeveLancarArgumentException()
        {
            // Arrange
            var userIdVazio = Guid.Empty;

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new IdempotencyRecord(userIdVazio, "POST", "key-123", "hash-abc", "{}", 201));
        }

        [Fact]
        public void Construtor_ComDadosValidos_DeveCriarRegistroCorretamente()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var record = new IdempotencyRecord(userId, "POST", "key-123", "hash-abc", "{}", 201);

            // Assert
            Assert.Equal(userId, record.UserId);
            Assert.Equal("POST", record.Operation);
            Assert.Equal("key-123", record.IdempotencyKey);
        }
    }
}