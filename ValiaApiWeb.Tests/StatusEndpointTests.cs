using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ValiaApiWeb.Tests
{
    public class StatusEndpointTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public StatusEndpointTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetStatus_DeveRetornar200OK()
        {
            // Arrange: Cria o cliente HTTP apontando para a API
            var client = _factory.CreateClient();

            // Act: Faz a requisição para o endpoint de status
            var response = await client.GetAsync("/api/status");

            // Assert: Verifica se retornou 200 OK
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}