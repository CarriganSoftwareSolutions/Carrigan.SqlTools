using Carrigan.Core.Extensions;
using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using System.Reflection;

namespace Carrigan.SqlTools.SqlServer;

public partial class SqlGenerator<T> : SqlGeneratorBase<T> where T : class
{
    /// <summary>
    /// The SQL dialect configuration used for generating database queries.
    /// </summary>
    protected override ISqlDialects Dialect { get; init; } = new SqlServerDialect();
    protected override HashSet<Type> SupportedTypes => DialectStatics.SupportedTypes;
    protected override SelectTagsBase GetAllSelectTags() =>
        SelectTagGenerator.GetAll<T>();
    protected override ColumnBase<T> GetColumn(PropertyName propertyName) =>
        new Column<T>(propertyName);
    protected override ColumnValueBase<T> GetColumnValue(ColumnInfo columnInfo, T entity) =>
        new ColumnValue<T>(columnInfo.PropertyName, columnInfo.PropertyInfo.GetValue(entity));
    protected override ColumnCollectionBase<T> GetColumnCollection(params IEnumerable<PropertyName> propertyNames) =>
        new ColumnCollection<T>(propertyNames);
    protected override OrderBysBase NewOrderBys() =>
        new OrderBys();
    protected override OrderByBase NewOrderByKey(PropertyName propertyName, SortDirectionEnum sortDirection) =>
        new OrderBy<T>(propertyName, sortDirection);

    public SqlGenerator() : base()
    {
        List<Exception> exceptions = [];

        GetColumnInfo(SupportedTypes)
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
