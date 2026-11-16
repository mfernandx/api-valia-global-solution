
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using System.Text;
using ValiaApi.Security;
using ValiaApi.Services;
using ValiaApiWeb.Data;
using ValiaApiWeb.Interfaces;
using ValiaApiWeb.Middleware;
using ValiaApiWeb.Security;
using ValiaApiWeb.Services;
using ValiaApiWeb.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// CONTROLLERS E OPENAPI
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler =
        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});


builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var problemDetailsFactory = context.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();

        var problem = problemDetailsFactory
            .CreateValidationProblemDetails(
                context.HttpContext,
                context.ModelState,
                statusCode: StatusCodes.Status400BadRequest,
                title: "Erro de validação",
                detail: "Um ou mais campos possuem valores inválidos.",
                instance: context.HttpContext.Request.Path
            );

        problem.Extensions["traceId"] = context.HttpContext.TraceIdentifier;

        return new BadRequestObjectResult(problem)
        {
            ContentTypes =
            {
                "application/problem+json"
            }
        };
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer(
        (document, context, cancellationToken) =>
        {
            document.Info.Title = "Valia API - Documentação OpenAPI";

            document.Info.Version = "v1";

            document.Info.Description = "Documentação da API Valia - Plataforma de conscientização, prevenção e identificação de possíveis violações de direitos trabalhistas.";

            return Task.CompletedTask;
        });

    options.AddDocumentTransformer<
        BearerSecuritySchemeTransformer>();
});


// BANCO DE DADOS - ORACLE
string connectionString = builder.Configuration.GetConnectionString("OracleConnection")
    ?? throw new InvalidOperationException("A conexão OracleConnection não foi configurada.");

builder.Services.AddDbContext<ValiaDbContext>(options =>
{
    options.UseOracle(connectionString);
});


// INJEÇÃO DE DEPENDÊNCIA - SERVICES
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

// AUTENTICAÇÃO JWT
string jwtSecret = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("A chave JWT não foi configurada.");

byte[] jwtKeyBytes = Convert.FromBase64String(jwtSecret);

if (jwtKeyBytes.Length < 32)
{
    throw new InvalidOperationException("A chave JWT deve possuir pelo menos 32 bytes.");
}

string jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("O emissor JWT não foi configurado.");

string jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("A audiência JWT não foi configurada.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;

        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,

                ValidateAudience = true,
                ValidAudience = jwtAudience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey =
                    new SymmetricSecurityKey(jwtKeyBytes),

                ValidateLifetime = true,
                RequireExpirationTime = true,

                ClockSkew = TimeSpan.FromSeconds(30),

                NameClaimType = "sub"
            };
    });


// AUTORIZAÇÃO
builder.Services.AddAuthorization();

builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();


// BUILD DA APLICAÇÃO
var app = builder.Build();


//Middleware de exceções
app.UseExceptionHandler();


// DOCUMENTAÇÃO - AMBIENTE DEVELOPMENT
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// MIDDLEWARES
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// ENDPOINTS
app.MapControllers();

// EXECUÇÃO
app.Run();


app.Run();

internal sealed class BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();

        if (authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
        {
            var requirements =
                new Dictionary<string, IOpenApiSecurityScheme>
                {
                    ["Bearer"] = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        In = ParameterLocation.Header,
                        BearerFormat = "JWT"
                    }
                };

            document.Components ??= new OpenApiComponents();

            document.Components.SecuritySchemes = requirements;
        }
    }
}

public partial class Program { }


