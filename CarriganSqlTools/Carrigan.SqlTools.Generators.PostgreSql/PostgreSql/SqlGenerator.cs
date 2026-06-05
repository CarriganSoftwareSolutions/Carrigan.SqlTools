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

/// <summary>
/// Represents the <see cref="SqlGenerator{T}"/> component.
/// </summary>
/// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
public partial class SqlGenerator<T> : SqlGeneratorBase<T> where T : class
{
    /// <summary>
    /// The SQL dialect configuration used for generating database queries.
    /// </summary>
    protected override ISqlDialects Dialect { get; init; } = new PostgreSqlDialect();
    /// <summary>
    /// Gets the SupportedTypes value.
    /// </summary>
    protected override HashSet<Type> SupportedTypes => Dialects.DialectStatics.SupportedTypes;
    /// <summary>
    /// Executes the GetAllSelectTags operation.
    /// </summary>
    /// <returns>The result of the GetAllSelectTags operation.</returns>
    protected override SelectTagsBase GetAllSelectTags() =>
        SelectTagGenerator.GetAll<T>();
    /// <summary>
    /// Executes the GetColumn operation.
    /// </summary>
    /// <param name="propertyName">The C# property name representing the SQL column or parameter.</param>
    /// <returns>The result of the GetColumn operation.</returns>
    protected override ColumnBase<T> GetColumn(PropertyName propertyName) =>
        new Column<T>(propertyName);
    /// <summary>
    /// Executes the GetColumnValue operation.
    /// </summary>
    /// <param name="columnInfo">The reflected column metadata for the model property.</param>
    /// <param name="entity">The model instance representing the SQL row or parameter set.</param>
    /// <returns>The result of the GetColumnValue operation.</returns>
    protected override ColumnValueBase<T> GetColumnValue(ColumnInfo columnInfo, T entity) =>
        new ColumnValue<T>(columnInfo.PropertyName, columnInfo.PropertyInfo.GetValue(entity));
    /// <summary>
    /// Executes the GetColumnCollection operation.
    /// </summary>
    /// <param name="propertyNames">The C# property names representing SQL columns or parameters.</param>
    /// <returns>The result of the GetColumnCollection operation.</returns>
    protected override ColumnCollectionBase<T> GetColumnCollection(params IEnumerable<PropertyName> propertyNames) =>
        new ColumnCollection<T>(propertyNames);
    /// <summary>
    /// Executes the NewOrderBys operation.
    /// </summary>
    /// <returns>The result of the NewOrderBys operation.</returns>
    protected override OrderBysBase NewOrderBys() =>
        new OrderBys();
    /// <summary>
    /// Executes the NewOrderByKey operation.
    /// </summary>
    /// <param name="propertyName">The C# property name representing the SQL column or parameter.</param>
    /// <param name="sortDirection">The SQL sort direction.</param>
    /// <returns>The result of the NewOrderByKey operation.</returns>
    protected override OrderByBase NewOrderByKey(PropertyName propertyName, SortDirectionEnum sortDirection) =>
        new OrderBy<T>(propertyName, sortDirection);

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlGenerator"/> class.
    /// </summary>
    /// <param name="encryption">The optional encryption service used for encrypted model properties.</param>
    public SqlGenerator(IEncryption? encryption = null) : base(encryption)
    {
    }
}
