using System.Data;
using System.Globalization;
using Dapper;

namespace MiniApi.Data
{
    // SQLite guarda Guid y DateTime como texto: Dapper necesita saber convertirlos.
    public static class DapperSetup
    {
        public static void Register()
        {
            SqlMapper.RemoveTypeMap(typeof(Guid));
            SqlMapper.RemoveTypeMap(typeof(DateTime));
            SqlMapper.AddTypeHandler(new GuidHandler());
            SqlMapper.AddTypeHandler(new UtcDateTimeHandler());
        }

        private sealed class GuidHandler : SqlMapper.TypeHandler<Guid>
        {
            public override void SetValue(IDbDataParameter parameter, Guid value) =>
                parameter.Value = value.ToString();

            public override Guid Parse(object value) => Guid.Parse((string)value);
        }

        private sealed class UtcDateTimeHandler : SqlMapper.TypeHandler<DateTime>
        {
            public override void SetValue(IDbDataParameter parameter, DateTime value) =>
                parameter.Value = value.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture);

            public override DateTime Parse(object value) =>
                DateTime.Parse((string)value, CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
        }
    }
}
