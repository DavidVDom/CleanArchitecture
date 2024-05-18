using CleanArchitecture.Application.Abstractions.Messaging;
using CleanArchitecture.Application.Exceptions;
using FluentValidation;
using MediatR;

namespace CleanArchitecture.Application.Abstractions.Behaviours
{
    // otro cross cutting concern, como el de logging
    public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IBaseCommand
    {
        // el paquete FluentValidation expone a la interface IValidator
        // realmente podemos aplicar múltiples validaciones al objeto TRequest que le llega, de ahí que sea una colección
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                // si no ha saltado ningún error de validación, que continúe
                // por ejemplo, observar las validaciones en ReservarAlquilerCommandValidator
                return await next();
            }

            // si ha saltado alguna validación
            var context = new ValidationContext<TRequest>(request);
            var validationErrors = _validators
                .Select(validators => validators.Validate(context))
                // los que sean de tipo error, no por ejemplo warnings
                .Where(validationResult => validationResult.Errors.Any())
                .SelectMany(validationResult => validationResult.Errors)
                // hacemos una proyección, es decir, los llevamos a otra estructura, mapeamos
                .Select(validationFailure => new ValidationError(
                    validationFailure.PropertyName,
                    validationFailure.ErrorMessage
                )).ToList();

            if (validationErrors.Any())
            {
                // usamos nuestra excepción personalizada
                throw new Exceptions.ValidationException(validationErrors);
            }

            return await next();
        }
    }
}
