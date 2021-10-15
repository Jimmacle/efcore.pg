using System;
using System.Globalization;
using Microsoft.EntityFrameworkCore.Storage;
using NpgsqlTypes;

namespace Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping
{
    public class NpgsqlTimestampTypeMapping : NpgsqlTypeMapping
    {
        public NpgsqlTimestampTypeMapping() : base("timestamp without time zone", typeof(DateTime), NpgsqlDbType.Timestamp) {}

        protected NpgsqlTimestampTypeMapping(RelationalTypeMappingParameters parameters)
            : base(parameters, NpgsqlDbType.Timestamp) {}

        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new NpgsqlTimestampTypeMapping(parameters);

        protected override string ProcessStoreType(RelationalTypeMappingParameters parameters, string storeType, string _)
            => parameters.Precision is null ? storeType : $"timestamp({parameters.Precision}) without time zone";

        protected override string GenerateNonNullSqlLiteral(object value)
            => $"TIMESTAMP '{GenerateLiteralCore(value)}'";

        protected override string GenerateEmbeddedNonNullSqlLiteral(object value)
            => $@"""{GenerateLiteralCore(value)}""";

        private string GenerateLiteralCore(object value)
        {
            var dateTime = (DateTime)value;

            if (!NpgsqlTypeMappingSource.LegacyTimestampBehavior && dateTime.Kind == DateTimeKind.Utc)
            {
                throw new InvalidCastException("'timestamp without time zone' literal cannot be generated for a UTC DateTime");
            }

            return dateTime.ToString("yyyy-MM-dd HH:mm:ss.FFFFFF", CultureInfo.InvariantCulture);
        }
    }
}
