namespace CleanArchitecture.Domain.Alquileres
{
    // strong type id, como en UserId
    public record AlquilerId(Guid Value)
    {
        public static AlquilerId New() => new(Guid.NewGuid());
    }
}
