using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using ValiaApiWeb.Data;
using Xunit;

namespace ValiaApiWeb.Tests
{
    public class WorkOccurrenceIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public WorkOccurrenceIntegrationTests(WebApplicationFactory<Program> factory)
        {
            // Configura a API para usar o banco em memória em vez do Oracle
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove o banco Oracle configurado
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ValiaDbContext>));

                    if (descriptor != null)
                        services.Remove(descriptor);

                    // Adiciona o banco em memória para o teste
                    services.AddDbContext<ValiaDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDb");
                    });
                });
            });
        }

        [Fact]
        public async Task Post_WorkOccurrence_SemToken_DeveRetornar401Unauthorized()
        {
            // Arrange
            var client = _factory.CreateClient();
            var request = new { /* body qualquer */ }; // Não importa o body, o teste vai falhar antes

            // Act
            // Tenta fazer o POST sem o token JWT
            var response = await client.PostAsJsonAsync("/api/WorkOccurrence", request);

            // Assert
            // Como o Controller tem [Authorize], deve retornar 401
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}