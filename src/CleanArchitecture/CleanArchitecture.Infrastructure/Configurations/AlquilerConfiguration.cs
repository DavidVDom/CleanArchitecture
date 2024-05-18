using CleanArchitecture.Domain.Alquileres;
using CleanArchitecture.Domain.Shared;
using CleanArchitecture.Domain.Users;
using CleanArchitecture.Domain.Vehiculos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Configurations
{
    internal sealed class AlquilerConfiguration : IEntityTypeConfiguration<Alquiler>
    {
        public void Configure(EntityTypeBuilder<Alquiler> builder)
        {
            builder.ToTable("alquileres");
            builder.HasKey(alquiler => alquiler.Id);

            // el id ahora es de tipo AlquilerId, para bbdd hay que convertirlo en Guid
            // en la conversión, primer parámetro el primitivo de bbdd y segundo nuestro object value de strong type id
            builder.Property(alquiler => alquiler.Id).
                HasConversion(alquilerId => alquilerId!.Value, value => new AlquilerId(value));

            // relación entre alquiler y su object value es 1 a 1
            builder.OwnsOne(alquiler => alquiler.PrecioPorPeriodo, precioBuilder =>
            {
                // y convertimos a primitivos sus valores
                precioBuilder.Property(moneda => moneda.TipoMoneda)
                .HasConversion(tipoMoneda => tipoMoneda.Codigo, codigo => TipoMoneda.FromCodigo(codigo!));
            });

            // precio mantenimiento es similar
            builder.OwnsOne(alquiler => alquiler.Mantenimiento, precioBuilder =>
            {
                precioBuilder.Property(moneda => moneda.TipoMoneda)
                .HasConversion(tipoMoneda => tipoMoneda.Codigo, codigo => TipoMoneda.FromCodigo(codigo!));
            });

            builder.OwnsOne(alquiler => alquiler.Accesorios, precioBuilder =>
            {
                precioBuilder.Property(moneda => moneda.TipoMoneda)
                .HasConversion(tipoMoneda => tipoMoneda.Codigo, codigo => TipoMoneda.FromCodigo(codigo!));
            });

            builder.OwnsOne(alquiler => alquiler.PrecioTotal, precioBuilder =>
            {
                precioBuilder.Property(moneda => moneda.TipoMoneda)
                .HasConversion(tipoMoneda => tipoMoneda.Codigo, codigo => TipoMoneda.FromCodigo(codigo!));
            });

            // duración en días del alquiler
            builder.OwnsOne(alquiler => alquiler.Duracion);

            // relación uno a muchos entre vehículo y alquiler
            // (un alquiler tiene un sólo vehículo, pero un vehículo puede tener muchos alquileres)
            builder.HasOne<Vehiculo>()
                .WithMany()
                .HasForeignKey(alquiler => alquiler.VehiculoId);

            // relación uno a muchos entre usuario y alquiler
            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(alquiler => alquiler.UserId);
        }
    }
}
