﻿using CleanArchitecture.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Repositories
{
    // repositorio genérico, como genérico tendrá tanto la clase del modelo como el Id, que será un object value en vez de un Guid (strong type id)
    internal abstract class Repository<TEntity, TEntityId>
        where TEntity : Entity<TEntityId>
        where TEntityId : class
    {
        protected readonly ApplicationDbContext DbContext;

        protected Repository(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public async Task<TEntity?> GetByIdAsync(TEntityId id, CancellationToken cancellationToken = default)
        {
            return await DbContext.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public void Add(TEntity entity)
        {
            DbContext.Add(entity);
        }
    }
}