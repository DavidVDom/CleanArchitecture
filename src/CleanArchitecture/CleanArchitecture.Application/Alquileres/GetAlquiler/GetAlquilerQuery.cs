using CleanArchitecture.Application.Abstractions.Messaging;

namespace CleanArchitecture.Application.Alquileres.GetAlquiler
{
    // le paso el id y me devuelve un objeto alquiler
    public sealed record GetAlquilerQuery(Guid AlguilerId) : IQuery<AlquilerResponse>;
}
