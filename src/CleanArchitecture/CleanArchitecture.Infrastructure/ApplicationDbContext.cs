using CleanArchitecture.Application.Exceptions;
using CleanArchitecture.Domain.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure
{
    public sealed class ApplicationDbContext : DbContext, IUnitOfWork
    {
        private readonly IPublisher _publisher;

        public ApplicationDbContext(DbContextOptions options, IPublisher publisher) : base(options)
        {
            _publisher = publisher;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // aquí se indica que se apliquen las configuraciones dentro del model builder
            // cuando el modelo esté configurado escaneará ese ensamblado encontrando cada una de las configuraciones de las entidades
            // y aplicará esos cambios al modelo de entity framework automáticamente, no hará falta aplicarlas una a una
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                // llama directamente a SaveChangesAsync de Entity Framework
                var result = await base.SaveChangesAsync(cancellationToken);

                // tras guardar publica todos los domain events con el IPublisher de MediatR
                await PublishDomainEventsAsync();

                return result;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // esto ocurre cuando se produce una violación de las reglas de la bbdd
                throw new ConcurrencyException("La excepción por concurrencia se disparó", ex);
            }
        }

        // publica todos los domain events
        private async Task PublishDomainEventsAsync()
        {
            var domainEvents = ChangeTracker
                .Entries<IEntity>() // todas las clases que hereden de Entity
                .Select(entry => entry.Entity)
                .SelectMany(entity =>
                {
                    var domainEvents = entity.GetDomainEvents();
                    entity.ClearDomainEvents();
                    return domainEvents;
                }).ToList();

            // una vez tengo todos los domain events, publico cada uno
            foreach (var domainEvent in domainEvents)
            {
                await _publisher.Publish(domainEvent);
            }
        }
    }
}
