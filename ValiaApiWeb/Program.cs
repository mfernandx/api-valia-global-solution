using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text.Json.Serialization;
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

var builder = WebApplication.CreateBuilder(args);

// ======================================================
// CONTROLLERS E SERIALIZAÇÃO JSON
// ======================================================

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler =
        ReferenceHandler.IgnoreCycles;

    options.JsonSerializerOptions.Converters.Add(
        new JsonStringEnumConverter()
    );
});

// ======================================================
// VALIDAÇÃO DAS REQUISIÇÕES
// ======================================================

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var factory = context.HttpContext.RequestServices
            .GetRequiredService<ProblemDetailsFactory>();

        var problem = factory.CreateValidationProblemDetails(
            context.HttpContext,
            context.ModelState,
            statusCode: StatusCodes.Status400BadRequest,
            title: "Erro de validação",
            detail: "Um ou mais campos possuem valores inválidos.",
            instance: context.HttpContext.Request.Path
        );

        problem.Extensions["traceId"] =
            context.HttpContext.TraceIdentifier;

        return new BadRequestObjectResult(problem)
        {
            ContentTypes =
            {
                "application/problem+json"
            }
        };
    };
});

// ======================================================
// DOCUMENTAÇÃO OPENAPI (SWASHBUCKLE) E SCALAR
// ======================================================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Valia API - Documentação OpenAPI",
        Version = "v1",
        Description =
            "API Valia - Plataforma de conscientização, prevenção e identificação de possíveis violações de direitos trabalhistas."
    });

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Informe o token JWT: Bearer {token}"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ======================================================
// BANCO DE DADOS - ORACLE
// ======================================================

string connectionString =
    builder.Configuration.GetConnectionString("OracleConnection")
    ?? throw new InvalidOperationException(
        "A conexão OracleConnection não foi configurada."
    );

builder.Services.AddDbContext<ValiaDbContext>(options =>
    options.UseOracle(
        connectionString,
        oracleOptions =>
            oracleOptions.UseOracleSQLCompatibility(
                OracleSQLCompatibility.DatabaseVersion21
            )
    )
);

// ======================================================
// CORS - ACESSO PELO FRONTEND / MOBILE
// ======================================================

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// ======================================================
// INJEÇÃO DE DEPENDÊNCIA - SERVICES
// ======================================================

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IEmploymentService, EmploymentService>();
builder.Services.AddScoped<IWorkShiftService, WorkShiftService>();
builder.Services.AddScoped<IWorkShiftAnalysisService, WorkShiftAnalysisService>();
builder.Services.AddScoped<IWorkOccurrenceService, WorkOccurrenceService>();
builder.Services.AddScoped<IEvidenceService, EvidenceService>();

builder.Services.AddSingleton<IFileStorageService, FileStorageService>();

builder.Services.AddScoped<IWorkConditionQuestionService, WorkConditionQuestionService>();
builder.Services.AddScoped<IWorkConditionAssessmentService, WorkConditionAssessmentService>();
builder.Services.AddScoped<ICommunityPostService, CommunityPostService>();
builder.Services.AddScoped<ICommunityCommentService, CommunityCommentService>();

// ======================================================
// AUTENTICAÇÃO JWT
// ======================================================

string jwtSecret = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "A chave JWT não foi configurada."
    );

byte[] jwtKeyBytes = Convert.FromBase64String(jwtSecret);

if (jwtKeyBytes.Length < 32)
{
    throw new InvalidOperationException(
        "A chave JWT deve possuir pelo menos 32 bytes."
    );
}

string jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException(
        "O emissor JWT não foi configurado."
    );

string jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException(
        "A audiência JWT não foi configurada."
    );

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(jwtKeyBytes),

            ValidateLifetime = true,
            RequireExpirationTime = true,

            ClockSkew = TimeSpan.FromSeconds(30),

            NameClaimType = "sub"
        };
    });

// ======================================================
// AUTORIZAÇÃO
// ======================================================

builder.Services.AddAuthorization();

// ======================================================
// RATE LIMITING - CONTROLE DE REQUISIÇÕES
// ======================================================

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter =
        PartitionedRateLimiter.Create<HttpContext, string>(
            httpContext =>
            {
                var ipAddress =
                    httpContext.Connection.RemoteIpAddress?.ToString()
                    ?? "unknown";

                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: ipAddress,
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 100,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }
                );
            }
        );

    options.RejectionStatusCode =
        StatusCodes.Status429TooManyRequests;

    options.OnRejected = (context, cancellationToken) =>
    {
        context.HttpContext.Response.Headers["Retry-After"] = "60";

        return ValueTask.CompletedTask;
    };
});

// ======================================================
// HEALTH CHECKS
// ======================================================

builder.Services
    .AddHealthChecks()
    .AddCheck<OracleDatabaseHealthCheck>(
        "oracle",
        tags: new[] { "ready" }
    );

// ======================================================
// TRATAMENTO GLOBAL DE EXCEÇÕES
// ======================================================

builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// ======================================================
// CONSTRUÇÃO DA APLICAÇÃO
// ======================================================

var app = builder.Build();

// ======================================================
// SEED - PERGUNTAS INICIAIS DO QUESTIONÁRIO
// ======================================================

if (app.Environment.IsDevelopment())
{
    var oracleConnection =
        app.Configuration.GetConnectionString("OracleConnection");

    if (!string.IsNullOrWhiteSpace(oracleConnection))
    {
        try
        {
            using var scope = app.Services.CreateScope();

            var context = scope.ServiceProvider
                .GetRequiredService<ValiaDbContext>();

            await WorkConditionQuestionSeeder.SeedAsync(context);

            app.Logger.LogInformation(
                "Seed das perguntas iniciais executado com sucesso."
            );
        }
        catch (Exception exception)
        {
            app.Logger.LogError(
                exception,
                "Não foi possível executar o Seed das perguntas iniciais."
            );
        }
    }
    else
    {
        app.Logger.LogWarning(
            "Seed ignorado: conexão Oracle ainda não configurada."
        );
    }
}

// ======================================================
// TRATAMENTO DE EXCEÇÕES
// ======================================================

app.UseExceptionHandler();

// ======================================================
// DOCUMENTAÇÃO OPENAPI (SWASHBUCKLE) E SCALAR
// ======================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Valia API v1");
        options.RoutePrefix = "swagger";
    });

    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Valia API")
            .WithTheme(ScalarTheme.BluePlanet)
            .WithDefaultHttpClient(
                ScalarTarget.CSharp,
                ScalarClient.HttpClient
            )
            .WithOpenApiRoutePattern("/swagger/v1/swagger.json");
    });
}

// ======================================================
// MIDDLEWARES
// ======================================================

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowFrontend");

app.UseRateLimiter();

app.UseAuthentication();

app.UseAuthorization();

// ======================================================
// MINIMAL API - STATUS DA APLICAÇÃO
// ======================================================

app.MapGet("/api/status", () =>
{
    return Results.Ok(new
    {
        status = "online",
        service = "Valia API",
        timestamp = DateTimeOffset.UtcNow
    });
})
.AllowAnonymous()
.WithName("GetApiStatus")
.WithTags("System")
.WithSummary("Verifica o funcionamento da API")
.WithDescription("Retorna o estado atual da aplicação Valia API.");

// ======================================================
// HEALTH CHECK - APLICAÇÃO
// ======================================================

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false,

    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(new
        {
            status = report.Status.ToString(),
            service = "Valia API"
        });
    }
})
.AllowAnonymous();

// ======================================================
// HEALTH CHECK - BANCO ORACLE
// ======================================================

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),

    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(new
        {
            status = report.Status.ToString(),

            database = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                description = entry.Value.Description
            })
        });
    }
})
.AllowAnonymous();

// ======================================================
// ENDPOINTS DOS CONTROLLERS EXISTENTES
// ======================================================

app.MapControllers();

// ======================================================
// CONFIGURAÇÃO DA PORTA PARA HOSPEDAGEM
// ======================================================

var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
app.Urls.Add($"http://0.0.0.0:{port}");



// ======================================================
// EXECUÇÃO
// ======================================================

app.Run();

public partial class Program { }