using CleanArchitecture.Application.Abstractions.Messaging;

namespace CleanArchitecture.Application.Users.LoginUser
{
    // un login exitoso devuelve un JWT
    public record LoginCommand(string Email, string Password) : ICommand<string>;
}
