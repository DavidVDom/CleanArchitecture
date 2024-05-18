using CleanArchitecture.Application.Abstractions.Clock;
using CleanArchitecture.Application.Abstractions.Data;
using CleanArchitecture.Application.Abstractions.Email;
using CleanArchitecture.Domain.Abstractions;
using CleanArchitecture.Domain.Alquileres;
using CleanArchitecture.Domain.Users;
using CleanArchitecture.Domain.Vehiculos;
using CleanArchitecture.Infrastructure.Clock;
using CleanArchitecture.Infrastructure.Data;
using CleanArchitecture.Infrastructure.Email;
using CleanArchitecture.Infrastructure.Repositories;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddTransient<IDateTimeProvider, DateTimeProvider>();
            services.AddTransient<IEmailService, EmailService>();

            var connectionString = configuration.GetConnectionString("Database") ?? throw new ArgumentNullException(nameof(configuration));
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                // estamos usando postgre
                // hacemos también uso de la librería efcore.namingconventions para transformar los nombres con snake case
                options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
            });

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IVehiculoRepository, VehiculoRepository>();
            services.AddScoped<IAlquilerRepository, AlquilerRepository>();

            // DI del unit of work, la interfaz requiere el servicio ApplicationDbContext desde IserviceProvider
            services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());

            // a la clase concreta de este singleton necesitamos pasarle como parámetro la cadena de conexión
            services.AddSingleton<ISqlConnectionFactory>(_ => new SqlConnectionFactory(connectionString));

            // preparamos para DI el type handler que hemos creado
            SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());

            return services;
        }
    }
}
