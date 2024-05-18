namespace CleanArchitecture.Api.Controllers.Alquileres
{
    // es básicamente un DTO
    public sealed record AlquilerReservaRequest(Guid VehiculoId, Guid UserId, DateOnly StartDate, DateOnly EndDate);
}
