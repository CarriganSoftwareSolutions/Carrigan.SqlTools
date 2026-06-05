using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.Tags;
using System.Data;
//IGNORE SPELLING: newid, unindexed

namespace Carrigan.SqlTools.SqlGenerators;

/// <summary>
/// Represents the <see cref="SqlGeneratorBase{T}"/> component.
/// </summary>
/// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
public abstract partial class SqlGeneratorBase<T>
{
    /// <summary>
    /// Builds the <c>VALUES</c> clause for a single SQL <c>INSERT</c> row,
    /// generating parameter placeholders for the specified columns.
    /// </summary>
    /// <param name="columns">
    /// The collection of <see cref="ColumnInfo"/> objects that identify the columns to insert.
    /// </param>
    /// <param name="i">
    /// Optional zero-based index used to append a unique index to each parameter name.
    /// If <c>null</c>, no index is appended and unindexed parameter names are used.
    /// </param>
    /// <returns>
    /// An <see cref="IEnumerable{SqlFragment}"/> representing the <c>VALUES</c> list for one row,
    /// for example <c>(@Column1, @Column2)</c> or <c>(@Column1_0, @Column2_0)</c>.
    /// </returns>
    private IEnumerable<ISqlFragment> GetInsertValueFragments(IEnumerable<ColumnInfo> columns, T entity) =>
    [
        new SqlFragmentText("("),
        ..columns
            .Select(column => GetSqlParameter(column, entity))
            .JoinFragments(new SqlFragmentText(", ")),
        new SqlFragmentText(")")
    ];

    /// <summary>
    /// Builds the <c>VALUES</c> clause for a single SQL <c>INSERT</c> row,
    /// generating parameter placeholders for the specified columns.
    /// </summary>
    /// <param name="columns">
    /// The collection of <see cref="ColumnInfo"/> objects that identify the columns to insert.
    /// </param>
    /// <param name="i">
    /// Optional zero-based index used to append a unique index to each parameter name.
    /// If <c>null</c>, no index is appended and unindexed parameter names are used.
    /// </param>
    /// <returns>
    /// A <see cref="SqlFragmentGroup"/> representing the <c>VALUES</c> list for one row,
    /// for example <c>(@Column1, @Column2)</c> or <c>(@Column1_0, @Column2_0)</c>.
    /// </returns>
    private SqlFragmentGroup GetEnumeratedInsertValueFragmentsGroup(IEnumerable<ColumnInfo> columns, T entity) =>
        new(GetInsertValueFragments(columns, entity));

    /// <summary>
    /// Builds the combined <c>VALUES</c> clause for a multi-row SQL <c>INSERT</c> statement,
    /// generating a parameterized value list for each entity.
    /// </summary>
    /// <param name="columns">
    /// The collection of <see cref="ColumnInfo"/> objects that identify the columns to insert.
    /// </param>
    /// <param name="entities">
    /// The collection of entity instances providing values for each row to insert.
    /// </param>
    /// <returns>
    /// An enumerable sequence of <see cref="ISqlFragment"/> representing the combined <c>VALUES</c> clause for all entities.
    /// </returns>
    private IEnumerable<ISqlFragment> GetEnumeratedInsertValueFragments(IEnumerable<ColumnInfo> columns, IEnumerable<T> entities) =>
        entities.Select((entity, index) => GetEnumeratedInsertValueFragmentsGroup(columns, entity))
            .JoinFragments(new SqlFragmentText(", "));

    /// <summary>
    /// Generates a SQL <c>INSERT</c> statement for one or more entities,
    /// relying on database default values for key (identity, <c>NEWID()</c>) properties.
    /// </summary>
    /// <param name="entities">
    /// One or more data model instances representing the new records to insert.
    /// </param>
    /// <returns>
    /// An <see cref="SqlQuery"/> representing the generated <c>INSERT</c> statement.
    /// </returns>
    /// <remarks>
    /// If the model has no non-key columns, <c>DEFAULT VALUES</c> is used.
    /// </remarks>
    /// <exception cref="NullReferenceException">
    /// Thrown if a column lacks a <see cref="ParameterTag"/> during parameter generation.
    /// This can surface indirectly from
    /// <see cref="GetSqlParameter(ColumnInfo, T)"/>.
    /// </exception>
    protected virtual SqlQuery BaseInsertAutoId(params IEnumerable<T> entities) =>
        BaseInsert
        (
            GetColumnCollection(GetGetColumnInfoLessKeys(SupportedTypes).Select(column => column.PropertyName)),
            GetColumnCollection(KeyColumnInfo.Select(column => column.PropertyName)),
            entities
        );

    /// <summary>
    /// Generates a SQL <c>INSERT</c> statement for one or more entities.
    /// </summary>
    /// <param name="insertColumnCollection">
    /// An optional collection specifying which columns to insert. If <c>null</c>,
    /// all mapped columns are included.
    /// </param>
    /// <param name="returnColumns">
    /// An optional collection specifying which columns’ inserted values should be returned.
    /// If <c>null</c>, no values are returned.
    /// </param>
    /// <param name="entities">
    /// One or more data model instances representing the new records to insert.
    /// </param>
    /// <returns>
    /// An <see cref="SqlQuery"/> representing the generated multi-row <c>INSERT</c> statement.
    /// </returns>
    /// <remarks>
    /// <list type="bullet">
    /// <item>
    /// <description>
    /// When only one entity is provided, unindexed parameter names are generated.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// When multiple entities are provided, parameter names are suffixed with the row index
    /// (for example, <c>@Name_0</c>).
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Only properties that are publicly readable and belong to accessible types
    /// are considered. Members not visible outside their defining assembly are ignored.
    /// </description>
    /// </item>
    /// </list>
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="entities"/> is empty.
    /// </exception>

    protected virtual SqlQuery BaseInsert(ColumnCollectionBase<T>? insertColumnCollection, ColumnCollectionBase<T>? returnColumns, params IEnumerable<T> entities)
    {
        if (entities.IsNullOrEmpty())
            throw new ArgumentException("No records provided.", nameof(entities));
        IEnumerable<ColumnInfo> insertTheseColumns = insertColumnCollection?.ColumnInfo ?? GetColumnInfo(SupportedTypes);

        IEnumerable<ISqlFragment> GetInsertIntoFragments()
        {
            IEnumerable<ISqlFragment> insertColumnFragments =
                insertTheseColumns
                    .Select(columnInfo => new SqlFragmentText(columnInfo.ColumnTag.ToSql(Dialect, false)))
                    .JoinFragments(new SqlFragmentText(", "));

            yield return new SqlFragmentText("INSERT INTO ");
            yield return Table;
            yield return new SqlFragmentText(" (");
            foreach (ISqlFragment fragment in insertColumnFragments)
                yield return fragment;
            yield return new SqlFragmentText(")");
        }

        IEnumerable<ISqlFragment> GetValuesFragments()
        {
            yield return new SqlFragmentText("VALUES ");
            if (entities.Count() > 1)
                foreach (ISqlFragment fragment in GetEnumeratedInsertValueFragments(insertTheseColumns, entities))
                    yield return fragment;
            else
                foreach (ISqlFragment fragment in GetInsertValueFragments(insertTheseColumns, entities.Single()))
                    yield return fragment;
        }

        IEnumerable<ISqlFragment> GetFinalFragments()
        {
            if (returnColumns.IsNotNullOrEmpty())
            {
                foreach (ISqlFragment fragment in Dialect.GetInsertReturningFragments<T>(GetInsertIntoFragments(), GetValuesFragments(), returnColumns.ColumnInfo))
                    yield return fragment;
            }
            else
            {

                foreach (ISqlFragment fragment in GetInsertIntoFragments())
                    yield return fragment;
                yield return ISqlFragment.Space;
                foreach (ISqlFragment fragment in GetValuesFragments())
                    yield return fragment;
                yield return ISqlFragment.Semicolon;
            }
        }

        return new SqlQuery(Dialect, CommandType.Text, GetFinalFragments());
    }
}
