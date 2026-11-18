using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using System.Text;
using System.Threading.RateLimiting;

using ValiaApi.Security;
using ValiaApi.Services;
using ValiaApiWeb.Data;
using ValiaApiWeb.HealthChecks;
using ValiaApiWeb.Interfaces;
using ValiaApiWeb.Middleware;
using ValiaApiWeb.Security;
using ValiaApiWeb.Services;
using ValiaApiWeb.Services.Interfaces;

namespace ValiaApiWeb.HealthChecks;

public class OracleDatabaseHealthCheck : IHealthCheck
{
    private readonly IServiceScopeFactory _scopeFactory;

    public OracleDatabaseHealthCheck(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();

            var dbContext = scope.ServiceProvider
                .GetRequiredService<ValiaDbContext>();

            bool canConnect = await dbContext.Database
                .CanConnectAsync(cancellationToken);

            if (canConnect)
            {
                return HealthCheckResult.Healthy(
                    "Conexão com o banco Oracle estabelecida.");
            }

            return HealthCheckResult.Unhealthy(
                "Não foi possível estabelecer conexão com o banco Oracle.");
        }
        catch (Exception)
        {
            return HealthCheckResult.Unhealthy(
                "O banco Oracle está indisponível ou não foi configurado.");
        }
    }
}
