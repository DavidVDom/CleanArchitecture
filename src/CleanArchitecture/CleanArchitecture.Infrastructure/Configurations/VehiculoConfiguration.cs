using CleanArchitecture.Domain.Shared;
using CleanArchitecture.Domain.Vehiculos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Configurations
{
    // IMPORTANTE: tener en cuenta que en las entidades de dominio sólo hemos reflejado las reglas de negocio, aquí será
    // donde crearemos las configuraciones específicas para entity para esas entidades, para migraciones, etc.
    internal sealed class VehiculoConfiguration : IEntityTypeConfiguration<Vehiculo>
    {
        public void Configure(EntityTypeBuilder<Vehiculo> builder)
        {
            builder.ToTable("vehiculos");
            builder.HasKey(vehiculo => vehiculo.Id);
            // conversión como en AlquilerConfiguration
            builder.Property(vehiculo => vehiculo.Id).
                HasConversion(vehiculoId => vehiculoId!.Value, value => new VehiculoId(value));

            // así como está, no se creará una nueva tabla dirección, sus campos se incluirán en la tabla vehículo
            builder.OwnsOne(vehiculo => vehiculo.Direccion);

            // Modelo es un object value, observar que se transforma a un tipo primitivo para postgre
            builder.Property(vehiculo => vehiculo.Modelo)
                .HasMaxLength(200)
                .HasConversion(modelo => modelo!.Value, value => new Modelo(value));

            builder.Property(vehiculo => vehiculo.Vin)
                .HasMaxLength(500)
                .HasConversion(vin => vin!.Value, value => new Vin(value));

            // el object value precio tiene también otros objects value, así que aquí hay que hacer más conversiones
            builder.OwnsOne(vehiculo => vehiculo.Precio, priceBuilder =>
            {
                priceBuilder.Property(moneda => moneda.TipoMoneda)
                .HasConversion(tipoMoneda => tipoMoneda.Codigo, codigo => TipoMoneda.FromCodigo(codigo!));
            });

            builder.OwnsOne(vehiculo => vehiculo.Mantenimiento, priceBuilder =>
            {
                priceBuilder.Property(moneda => moneda.TipoMoneda)
                .HasConversion(tipoMoneda => tipoMoneda.Codigo, codigo => TipoMoneda.FromCodigo(codigo!));
            });

            builder.Property<uint>("Version").IsRowVersion();
        }
    }
}
