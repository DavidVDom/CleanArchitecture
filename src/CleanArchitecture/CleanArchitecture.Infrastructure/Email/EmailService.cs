using CleanArchitecture.Application.Abstractions.Email;

namespace CleanArchitecture.Infrastructure.Email
{
    internal sealed class EmailService : IEmailService
    {
        public Task SendAsync(CleanArchitecture.Domain.Users.Email recipient, string subject, string body)
        {
            // TODO: implementación de la lógica con el provider que sea
            return Task.CompletedTask;
        }
    }
}
