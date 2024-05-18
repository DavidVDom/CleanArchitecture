using CleanArchitecture.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Repositories
{
    internal sealed class UserRepository : Repository<User, UserId>, IUserRepository
    {
        public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        // en el padre abstracto genérico ya están definidos algunos métodos que expone la interfaz

        public async Task<User> GetByEmailAsync(Domain.Users.Email email, CancellationToken cancellationToken = default)
        {
            // recordar que al construir la query transformará el object value en un tipo primitivo
            return await DbContext.Set<User>().FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
        }

        public async Task<bool> IsUserExists(Domain.Users.Email email, CancellationToken cancellationToken = default)
        {
            return await DbContext.Set<User>().AnyAsync(x => x.Email == email);
        }
    }
}
