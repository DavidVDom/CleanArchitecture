namespace CleanArchitecture.Domain.Users;

public interface IUserRepository
{

    Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default);

    void Add(User user);

    Task<User> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);

    // validamos únicamente por email para este ejemplo
    Task<bool> IsUserExists(Email email, CancellationToken cancellationToken = default);
}