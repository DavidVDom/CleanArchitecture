namespace CleanArchitecture.Application.Exceptions
{
    public sealed class ConcurrencyException : Exception
    {
        // concurrencia optimista
        // se usará en SaveChangesAsync de ApplicationDbContext, o en ReservarAlquilerCommandHandler
        // por ejemplo, dos personas intentan alquilar el mismo vehículo en un intervalo de tiempo que coincide en parte y se produce
        // una excepción de concurrencia
        // la concurrencia se checkea con una versión, observar que en VehiculoConfiguracion se añade un campo Version (ver ahí definición de IsRowVersion)
        public ConcurrencyException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
