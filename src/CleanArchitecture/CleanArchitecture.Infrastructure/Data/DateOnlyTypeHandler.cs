using Dapper;
using System.Data;

namespace CleanArchitecture.Infrastructure.Data
{
    // postgre no entiende el tipo DateOnly que estamos usando en algunas propiedades
    // usamos SqlMapper.TypeHandler de Dapper
    internal sealed class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
    {
        public override DateOnly Parse(object value) => DateOnly.FromDateTime((DateTime)value);

        public override void SetValue(IDbDataParameter parameter, DateOnly value)
        {
            parameter.DbType = DbType.Date;
            parameter.Value = value;
        }
    }
}
