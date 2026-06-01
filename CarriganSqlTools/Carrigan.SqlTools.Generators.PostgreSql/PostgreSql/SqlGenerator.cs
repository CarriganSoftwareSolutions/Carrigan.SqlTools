using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.PostgreSql;

public partial class SqlGenerator<T> : SqlGeneratorBase<T> where T : class
{
    /// <summary>
    /// The SQL dialect configuration used for generating database queries.
    /// </summary>
    protected override ISqlDialects Dialect { get; init; } = new PostgreSqlDialect();
    protected override HashSet<Type> SupportedTypes => Dialects.DialectStatics.SupportedTypes;
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

    public SqlGenerator(IEncryption? encryption = null) : base(encryption)
    {
    }
}
