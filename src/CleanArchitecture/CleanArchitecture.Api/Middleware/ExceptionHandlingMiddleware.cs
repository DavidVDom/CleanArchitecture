using CleanArchitecture.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        // capturará las excepciones de la aplicación

        // request delegate que contiene las variables y el flujo de la aplicación
        private readonly RequestDelegate _next;

        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // si no hay excepciones que continúe
                await _next(context);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Ocurrió una excepción: {Message}", exception.Message);
                var exceptionDetails = GetExceptionDetails(exception);
                var problemDetails = new ProblemDetails
                {
                    Status = exceptionDetails.Status,
                    Type = exceptionDetails.Type,
                    Title = exceptionDetails.Title,
                    Detail = exceptionDetails.Detail
                };

                if (exceptionDetails.Errors is not null)
                {
                    // en este caso el error es un tipo de validación
                    problemDetails.Extensions["errors"] = exceptionDetails.Errors;
                }

                context.Response.StatusCode = exceptionDetails.Status;

                await context.Response.WriteAsJsonAsync(problemDetails);
            }
        }

        private static ExceptionDetails GetExceptionDetails(Exception exception)
        {
            // podríamos evaluar otros tipos de excepciones también

            return exception switch
            {
                ValidationException validationException => new ExceptionDetails(
                    StatusCodes.Status400BadRequest,
                    "ValidationFailure",
                    "Validacion de Error",
                    "han ocurrido uno o mas errores de validacion",
                    validationException.Errors
                ),
                _ => new ExceptionDetails(
                    StatusCodes.Status500InternalServerError,
                    "ServerError",
                    "Error de Servidor",
                    "Ha ocurrido un error inesperado",
                    null
                )
            };
        }

        internal record ExceptionDetails(int Status, string Type, string Title, string Detail, IEnumerable<object>? Errors);
    }
}
