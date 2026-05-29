using Carrigan.Core.Extensions;
using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.SqlGenerators;
using System.Reflection;

namespace Carrigan.SqlTools.SqlServer;

public partial class SqlGenerator<T> : SqlGeneratorBase<T> where T : class
{
    /// <summary>
    /// The SQL dialect configuration used for generating database queries.
    /// </summary>
    protected override ISqlDialects Dialect { get; init; } = new SqlServerDialect();

    public SqlGenerator() : base()
    {
        List<Exception> exceptions = [];

        ColumnInfo
            .Select(static column => new Tuple<PropertyInfo, SqlTypeAttribute?>(column.PropertyInfo, SqlTypeAttribute.GetSqlTypeAttribute(column.PropertyInfo)))
            .Select(static tuple => SqlTypeMismatchException.Validate(tuple.Item1, tuple.Item2))
            .ForEach(sqlTypeMismatchException =>
            {
                if (sqlTypeMismatchException is not null)
                    exceptions.Add(sqlTypeMismatchException);
            });

        if(exceptions.Count ==  1)
        {
            throw exceptions.Single();
        }
        else if(exceptions.Count > 1)
        {
            throw new AggregateException(exceptions);
        }
    }

    public SqlGenerator(IEncryption? encryption) : base(encryption)
    {
    }
}
