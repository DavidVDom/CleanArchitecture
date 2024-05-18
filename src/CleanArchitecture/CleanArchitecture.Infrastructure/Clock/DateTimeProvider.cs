using CleanArchitecture.Application.Abstractions.Clock;

namespace CleanArchitecture.Infrastructure.Clock
{
    // interna, sólo necesitamos que sea pública la interfaz
    internal sealed class DateTimeProvider : IDateTimeProvider
    {
        public DateTime CurrentTime => DateTime.UtcNow;
    }
}
