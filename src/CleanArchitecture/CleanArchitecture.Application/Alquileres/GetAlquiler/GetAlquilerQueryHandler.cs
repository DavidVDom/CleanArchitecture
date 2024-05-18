using CleanArchitecture.Application.Abstractions.Messaging;
using CleanArchitecture.Application.Abstractions.Data;
using Dapper;

namespace CleanArchitecture.Application.Alquileres.GetAlquiler
{
    // internal porque el query handler no será expuesto a componentes externos, el único que se expone es el objeto Query,
    // ya que lo necesitarán los controles
    internal sealed class GetAlquilerQueryHandler : IQueryHandler<GetAlquilerQuery, AlquilerResponse>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public GetAlquilerQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<CleanArchitecture.Domain.Abstractions.Result<AlquilerResponse>> Handle(
            GetAlquilerQuery request,
            CancellationToken cancellationToken)
        {
            // para commands usaremos entity framework, pero para consultas usaremos Dapper
            using var connection = _sqlConnectionFactory.CreateConnection();

            // se trabajará con postgresql, que es case sensitive, no vale "select..."
            // alias para mapear contra AlquilerResponse
            var sql = """
                SELECT
                    id AS Id,
                    vehiculo_id AS VehiculoId,
                    user_id AS UserId,
                    status AS Status,
                    precio_por_periodo AS PrecioAlquiler,
                    precio_por_periodo_tipo_moneda AS TipoMonedaAlquiler,
                    precio_mantenimiento AS PrecioMantenimiento,
                    precio_mantenimiento_tipo_moneda AS TipoMonedaMantenimiento,
                    precio_accesorios AS AccesoriosPrecio,
                    precio_accesorios_tipo_moneda AS TipoMonedaAccesorio,
                    precio_total AS PrecioTotal,
                    precio_total_tipo_moneda AS PrecioTotalTipoMoneda,
                    duracion_inicio AS DuracionInicio,
                    duracion_final AS DuracionFinal,
                    fecha_creacion AS FechaCreacion
                FROM alquileres WHERE id = @AlguilerId
                """;

            // la query sql y los parámetros en un objeto anónimo
            var alquiler = await connection.QueryFirstOrDefaultAsync<AlquilerResponse>(
                sql,
                new { request.AlguilerId });

            return alquiler!;
        }
    }
}
