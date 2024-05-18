using CleanArchitecture.Api.Middleware;
using CleanArchitecture.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Api.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        // método de extensión de IApplicationBuilder que usaremos para las migraciones
        public static async void ApplyMigration(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                // usamos un service provider para ir recuperando nuestras clases concretas
                var service = scope.ServiceProvider;
                var loggerFactory = service.GetRequiredService<ILoggerFactory>();

                try
                {
                    var context = service.GetRequiredService<ApplicationDbContext>();
                    await context.Database.MigrateAsync();
                }
                catch (Exception ex)
                {
                    // sobre Program, porque será la clase que llame a ese método
                    var logger = loggerFactory.CreateLogger<Program>();
                    logger.LogError(ex, "Error en migración");
                }
            }
        }

        public static void UseCustomExceptionHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
