using CleanArchitecture.Domain.Abstractions;
using CleanArchitecture.Domain.Users.Events;

namespace CleanArchitecture.Domain.Users;

public sealed class User : Entity<UserId>
{
    private User() { }

    private User(
        //Guid id, // con strong type id usaremos nuestro object value UserId en lugar del tipo primitivo Guid
        UserId id,
        Nombre nombre,
        Apellido apellido,
        Email email,
        PasswordHash passwordHash
        ) : base(id)
    {
        Nombre = nombre;
        Apellido = apellido;
        Email = email;
        PasswordHash = passwordHash;
    }

    public Nombre? Nombre { get; private set; }
    public Apellido? Apellido { get; private set; }
    public Email? Email { get; private set; }
    public PasswordHash? PasswordHash { get; private set; }

    public static User Create(
        Nombre nombre,
        Apellido apellido,
        Email email,
        PasswordHash passwordHash
    )
    {
        //var user = new User(Guid.NewGuid(), nombre, apellido, email);
        var user = new User(UserId.New(), nombre, apellido, email, passwordHash);
        user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id!));
        return user;
    }

}