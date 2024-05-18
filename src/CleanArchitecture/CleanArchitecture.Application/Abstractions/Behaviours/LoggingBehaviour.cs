using CleanArchitecture.Application.Abstractions.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Abstractions.Behaviours
{
    // usamos IPipelineBehavior de MediatR, con el constraint donde el genérico TRequest implementa IBaseCommand,
    // ya que los loggins escriben datos.
    // Se podría usar serilog o cualquier otra librería pero en este caso se usará el logging propio de MS (Microsoft.Extensions.Logging.Abstractions)

    // esto sería con cross cutting concern (otro sería el de validation), no sé muy bien por qué lo mete en la capa de application
    // y no en un proyecto cross cutting
    public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IBaseCommand
    {
        private readonly ILogger<TRequest> _logger;

        public LoggingBehaviour(ILogger<TRequest> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            // capturará todos los requests de tipo command que envíe el cliente, observar que el constraint restring a requests de tipo IBaseCommand
            var name = request.GetType().Name;
            try
            {
                _logger.LogInformation($"Ejecutando el command request {name}");
                var result = await next(); // si no hay error, imprimirá el siguiente mensaje
                _logger.LogInformation($"El command request {name} se ejecutó exitosamente");

                // y que continúe la secuencia del programa
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"El command {name} tuvo errores");
                throw;
            }
        }
    }
}
