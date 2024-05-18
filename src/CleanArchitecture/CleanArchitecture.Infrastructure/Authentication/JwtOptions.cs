namespace CleanArchitecture.Infrastructure.Authentication
{
    public class JwtOptions
    {
        // se inicializarán con los valores de configuración en appsettings.json
        public string? Issuer { get; init; }
        public string? Audience { get; init; }
        public string? SecretKey { get; init; }
    }
}
