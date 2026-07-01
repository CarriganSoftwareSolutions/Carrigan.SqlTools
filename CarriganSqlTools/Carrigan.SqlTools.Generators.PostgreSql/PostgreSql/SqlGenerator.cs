using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.GroupByClause;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.PostgreSql;

/// <summary>
/// Generates SQL queries for the specified model type.
/// </summary>
/// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
public partial class SqlGenerator<T> : SqlGeneratorBase<T> where T : class
{
    /// <summary>
    /// The SQL dialect configuration used for generating database queries.
    /// </summary>
    protected override ISqlDialects Dialect { get; init; } = new PostgreSqlDialect();
    /// <summary>
    /// Gets the CLR types supported by this SQL dialect.
    /// </summary>
    protected override HashSet<Type> SupportedTypes => Dialects.DialectStatics.SupportedTypes;
    /// <summary>
    /// Gets all selectable tags for the model type.
    /// </summary>
    /// <returns>The selectable tags resolved from the model type.</returns>
    protected override SelectTagsBase GetAllSelectTags() =>
        SelectTagGenerator.GetAll<T>();

    /// <summary>
    /// Gets selectable tags for the supplied GROUP BY columns.
    /// </summary>
    /// <param name="groupBys">The group-by columns to project.</param>
    /// <returns>The select tags resolved from the group-by columns.</returns>
    protected override SelectTagsBase GetSelectTags(GroupBysBase groupBys) =>
        SelectTagGenerator.GetMany(groupBys);
    /// <summary>
    /// Creates a dialect-specific column expression for a model property.
    /// </summary>
    /// <param name="propertyName">The C# property name representing the SQL column or parameter.</param>
    /// <returns>A dialect-specific column expression for the requested property.</returns>
    protected override ColumnBase<T> GetColumn(PropertyName propertyName) =>
        new Column<T>(propertyName);
    /// <summary>
    /// Creates a dialect-specific column-value expression from reflected column metadata and an entity instance.
    /// </summary>
    /// <param name="columnInfo">The reflected column metadata for the model property.</param>
    /// <param name="entity">The model instance representing the SQL row or parameter set.</param>
    /// <returns>A dialect-specific column-value expression containing the property value from <paramref name="entity"/>.</returns>
    protected override ColumnValueBase<T> GetColumnValue(ColumnInfo columnInfo, T entity) =>
        new ColumnValue<T>(columnInfo.PropertyName, columnInfo.PropertyInfo.GetValue(entity));
    /// <summary>
    /// Creates a dialect-specific collection of columns from property names.
    /// </summary>
    /// <param name="propertyNames">The C# property names representing SQL columns or parameters.</param>
    /// <returns>A dialect-specific column collection containing the requested properties.</returns>
    protected override ColumnCollectionBase<T> GetColumnCollection(params IEnumerable<PropertyName> propertyNames) =>
        new ColumnCollection<T>(propertyNames);
    /// <summary>
    /// Creates an empty dialect-specific ORDER BY collection.
    /// </summary>
    /// <returns>An empty dialect-specific ORDER BY collection.</returns>
    protected override OrderBysBase NewOrderBys() =>
        new OrderBys();
    /// <summary>
    /// Creates a dialect-specific ORDER BY item for a model property.
    /// </summary>
    /// <param name="propertyName">The C# property name representing the SQL column or parameter.</param>
    /// <param name="sortDirection">The SQL sort direction.</param>
    /// <returns>A dialect-specific ORDER BY item for the requested property and sort direction.</returns>
    protected override OrderByBase NewOrderByKey(PropertyName propertyName, SortDirectionEnum sortDirection) =>
        new OrderBy<T>(propertyName, sortDirection);

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlGenerator{T}"/> class.
    /// </summary>
    /// <param name="encryption">The optional encryption service used for encrypted model properties.</param>
    public SqlGenerator(IEncryption? encryption = null) : base(encryption)
    {
    }
}
