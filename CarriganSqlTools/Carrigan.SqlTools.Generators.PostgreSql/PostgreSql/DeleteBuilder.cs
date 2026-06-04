using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
//TODO: QUery Builder classes need to be reevaluated IEncryption support.
namespace Carrigan.SqlTools.PostgreSql;

/// <summary>
/// Represents the options used to build a DELETE query for the specified model type.
/// </summary>
/// <typeparam name="T">The model type being updated.</typeparam>
/// <typeparam name="joinsT">The model type used as the starting point for the join collection. This type should represent one of the source tables in the USING clause.</typeparam>
/// <remarks>
/// For PostgreSQL, <typeparamref name="joinsT" /> should represent one of the source tables in the DELETE USING clause.
/// </remarks>
public sealed record DeleteBuilder<T, joinsT> : QueryBuilders.DeleteBuilderBase<T, joinsT>, IQueryBuilder
    where T : class
    where joinsT : class
{
    private readonly SqlGenerator<T> SqlGenerator = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteBuilder"/> class.
    /// </summary>
    /// <param name="encryption">The optional encryption service used for encrypted model properties.</param>
    public DeleteBuilder(IEncryption? encryption = null) =>
        SqlGenerator = new (encryption);

    /// <summary>
    /// Gets or sets the tables to include in the USING clause.
    /// </summary>
    public IEnumerable<TableTag>? Usings { get; set; }

    /// <summary>
    /// Returns a copy of the current query with the specified USING tables.
    /// </summary>
    /// <param name="usings">The tables to include in the USING clause.</param>
    /// <returns>A new query instance with the specified USING tables.</returns>
    /// <remarks>
    /// For PostgreSQL, <typeparamref name="joinsT" /> should represent one of the source tables in the DELETE USING clause.
    /// </remarks>
    public DeleteBuilder<T, joinsT> WithUsings(IEnumerable<TableTag>? usings) =>
        this with { Usings = usings };

    /// <summary>
    /// Builds a SQL query from the current builder state.
    /// </summary>
    /// <returns>The result of the AsSqlQuery operation.</returns>
    public SqlQuery AsSqlQuery() =>
        SqlGenerator.Delete(this);
}



/// <summary>
/// Represents the options used to build a DELETE query for the specified model type.
/// </summary>
/// <typeparam name="T">The model type being updated.</typeparam>
public sealed record DeleteBuilder<T> : QueryBuilders.DeleteBuilderBase<T, T>, IQueryBuilder
    where T : class
{
    private const string JoinsNotSupportedMessage =
    "PostgreSQL DELETE joins are rooted from a table in the USING clause, not from the table being deleted. " +
    "This one-type DeleteBuilder<T> exists only for PostgreSQL DELETE statements that do not use Joins. " +
    "The inherited Joins member must exist because DeleteBuilderBase<T, T> defines it, but using Joins<T> here would incorrectly require the join chain to start with the deleted type. " +
    "Use DeleteBuilder<T, joinsT> when a PostgreSQL DELETE statement requires joins.";

    private readonly SqlGenerator<T> SqlGenerator = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteBuilder"/> class.
    /// </summary>
    /// <param name="encryption">The optional encryption service used for encrypted model properties.</param>
    public DeleteBuilder(IEncryption? encryption = null) =>
        SqlGenerator = new(encryption);

    /// <summary>
    /// Gets or sets the tables to include in the USING clause.
    /// </summary>
    public IEnumerable<TableTag>? Usings { get; set; }

    [Obsolete(JoinsNotSupportedMessage, true)]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
    /// <summary>
    /// Gets the Joins value.
    /// </summary>
    public override Joins<T>? Joins { get; set; }
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member


    [Obsolete(JoinsNotSupportedMessage, true)]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
    /// <summary>
    /// Executes the WithJoins operation.
    /// </summary>
    /// <param name="joins">The SQL joins used by the query.</param>
    /// <returns>The result of the WithJoins operation.</returns>
    public override DeleteBuilder<T> WithJoins(Joins<T>? joins) =>
        this with { Joins = joins };
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member

    /// <summary>
    /// Builds a SQL query from the current builder state.
    /// </summary>
    /// <returns>The result of the AsSqlQuery operation.</returns>
    public SqlQuery AsSqlQuery() =>
        SqlGenerator.Delete(this);
}
