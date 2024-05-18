using CleanArchitecture.Domain.Abstractions;
using CleanArchitecture.Domain.Alquileres.Events;
using CleanArchitecture.Domain.Shared;
using CleanArchitecture.Domain.Users;
using CleanArchitecture.Domain.Vehiculos;

namespace CleanArchitecture.Domain.Alquileres;

public sealed class Alquiler : Entity<AlquilerId>
{
    // tener en cuenta que para la configuraci�n estamos usando clases de configuraci�n en infrastructure, con lo cual al crear
    // migraciones va a dar problemas porque buscar� un constructor, por lo tanto, creamos aqu� un constructor privado (tambi�n debe tener
    // uno protected en su clase padre Entity) para que no de esos problemas al ejecutar migraciones
    private Alquiler() { }

    private Alquiler(
        AlquilerId id,
        VehiculoId vehiculoId,
        UserId userId,
        DateRange duracion,
        Moneda precioPorPeriodo,
        Moneda mantenimiento,
        Moneda accesorios,
        Moneda precioTotal,
        AlquilerStatus status,
        DateTime fechaCreacion

        ) : base(id)
    {
        VehiculoId = vehiculoId;
        UserId = userId;
        Duracion = duracion;
        PrecioPorPeriodo = precioPorPeriodo;
        Mantenimiento = mantenimiento;
        Accesorios = accesorios;
        PrecioTotal = precioTotal;
        Status = status;
        FechaCreacion = fechaCreacion;
    }

    public VehiculoId? VehiculoId { get; private set; }

    public UserId? UserId { get; private set; }
    public Moneda? PrecioPorPeriodo { get; private set; }
    public Moneda? Mantenimiento { get; private set; }
    public Moneda? Accesorios { get; private set; }
    public Moneda? PrecioTotal { get; private set; }
    public AlquilerStatus Status { get; private set; }

    public DateRange? Duracion { get; private set; }

    public DateTime? FechaCreacion { get; private set; }
    public DateTime? FechaConfirmacion { get; private set; }

    public DateTime? FechaDenegacion { get; private set; }
    public DateTime? FechaCompletado { get; private set; }

    public DateTime? FechaCancelacion { get; private set; }


    public static Alquiler Reservar(
      Vehiculo vehiculo,
      UserId userId,
      DateRange duracion,
      DateTime fechaCreacion,
      PrecioService precioService
    )
    {
        var precioDetalle = precioService.CalcularPrecio(
            vehiculo,
            duracion
        );


        var alquiler = new Alquiler(
            //Guid.NewGuid(),
            AlquilerId.New(),
            vehiculo.Id!,
            userId,
            duracion,
            precioDetalle.PrecioPorPeriodo,
            precioDetalle.Mantenimiento,
            precioDetalle.Accesorios,
            precioDetalle.PrecioTotal,
            AlquilerStatus.Reservado,
            fechaCreacion
        );

        // publicamos un evento en un queue, y otro componente a futuro consumir� ese evento y har� algo
        // cada vez que genero un alquiler con estado de reservar, se env�a ese mensaje
        alquiler.RaiseDomainEvent(new AlquilerReservadoDomainEvent(alquiler.Id!));

        vehiculo.FechaUltimaAlquiler = fechaCreacion;

        return alquiler;
    }


    public Result Confirmar(DateTime utcNow)
    {
        if (Status != AlquilerStatus.Reservado)
        {
            return Result.Failure(AlquilerErrors.NotReserved);
        }

        Status = AlquilerStatus.Confirmado;
        FechaConfirmacion = utcNow;

        RaiseDomainEvent(new AlquilerConfirmadoDomainEvent(Id!));
        return Result.Success();
    }


    public Result Rechazar(DateTime utcNow)
    {
        if (Status != AlquilerStatus.Reservado)
        {
            return Result.Failure(AlquilerErrors.NotReserved);
        }

        Status = AlquilerStatus.Rechazado;
        FechaDenegacion = utcNow;
        RaiseDomainEvent(new AlquilerRechazadoDomainEvent(Id!));


        return Result.Success();
    }

    public Result Cancelar(DateTime utcNow)
    {
        if (Status != AlquilerStatus.Confirmado)
        {
            return Result.Failure(AlquilerErrors.NotConfirmado);
        }

        var currentDate = DateOnly.FromDateTime(utcNow);

        if (currentDate > Duracion!.Inicio)
        {
            return Result.Failure(AlquilerErrors.AlreadyStarted);
        }


        Status = AlquilerStatus.Cancelado;
        FechaCancelacion = utcNow;
        RaiseDomainEvent(new AlquilerCanceladoDomainEvent(Id!));


        return Result.Success();
    }


    public Result Completar(DateTime utcNow)
    {
        if (Status != AlquilerStatus.Confirmado)
        {
            return Result.Failure(AlquilerErrors.NotConfirmado);
        }

        Status = AlquilerStatus.Completado;
        FechaCompletado = utcNow;
        RaiseDomainEvent(new AlquilerCompletadoDomainEvent(Id!));


        return Result.Success();
    }



}