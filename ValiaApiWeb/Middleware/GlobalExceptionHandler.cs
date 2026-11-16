
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ValiaApiWeb.Domain.Exceptions;

namespace ValiaApiWeb.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            int statusCode = exception switch
            {
                ArgumentException => StatusCodes.Status400BadRequest,

                KeyNotFoundException => StatusCodes.Status404NotFound,

                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,

                BusinessConflictException => StatusCodes.Status409Conflict,

                _ => StatusCodes.Status500InternalServerError
            };

            if (statusCode >= 500)
            {
                _logger.LogError(exception, "Erro inesperado ao processar a requisição.");
            }
            else
            {
                _logger.LogWarning("Erro na requisição: {ErrorType}", exception.GetType().Name);
            }

            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = statusCode switch
                {
                    400 => "Requisição inválida",
                    401 => "Não autorizado",
                    404 => "Recurso não encontrado",
                    409 => "Conflito na operação",
                    _ => "Erro interno do servidor"
                },
                Detail = statusCode == 500
                    ? "Ocorreu um erro inesperado. Tente novamente mais tarde."
                    : exception.Message, Instance = httpContext.Request.Path
            };

            problem.Extensions["traceId"] = httpContext.TraceIdentifier;

            httpContext.Response.StatusCode = statusCode;

            await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

            return true;
        }
    }
}

