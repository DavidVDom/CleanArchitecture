using CleanArchitecture.Domain.Alquileres;
using CleanArchitecture.Application.Abstractions.Behaviours;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            // con esa coma en <,> le estamos indicando que esa clase tiene dos elementos generic a inyectarse, TRequest y TResponse
            configuration.AddOpenBehavior(typeof(LoggingBehaviour<,>));
            configuration.AddOpenBehavior(typeof(ValidationBehaviour<,>));
        });

        // escanea todas las clases de tipo FluentValidation para inyectarlas
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddTransient<PrecioService>();

        return services;
    }
}