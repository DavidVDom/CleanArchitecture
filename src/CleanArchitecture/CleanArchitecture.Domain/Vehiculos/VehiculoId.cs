namespace CleanArchitecture.Domain.Vehiculos
{
    // strong type id, como en UserId
    public record VehiculoId(Guid Value)
    {
        public static VehiculoId New() => new(Guid.NewGuid());
    }
}
