using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.PostgreSql;

/// <summary>
/// Builds UPDATE query options for the specified model type.
/// </summary>
/// <typeparam name="T">The model type being updated.</typeparam>
/// <typeparam name="joinsT">The model type used as the starting point for the join collection. This type should represent one of the source tables in the FROM clause.</typeparam>
/// <remarks>
/// For PostgreSQL, <typeparamref name="joinsT" /> should represent one of the source tables in the UPDATE FROM clause.
/// </remarks>
public sealed record UpdateBuilder<T, joinsT> : QueryBuilders.UpdateBuilderBase<T, joinsT>, IQueryBuilder
    where T : class
    where joinsT : class
{

    /// <summary>
    /// Generates SQL for the builder state.
    /// </summary>
    private readonly SqlGenerator<T> SqlGenerator = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateBuilder{T, joinsT}"/> class.
    /// </summary>
    /// <param name="encryption">The optional encryption service used for encrypted model properties.</param>
    public UpdateBuilder(IEncryption? encryption = null) =>
        SqlGenerator = new(encryption);

    /// <summary>
    /// Gets or sets the tables to include in the FROM clause.
    /// </summary>
    /// <remarks>
    /// For PostgreSQL, <typeparamref name="joinsT" /> should represent one of the source tables in the UPDATE FROM clause.
    /// </remarks>
    public IEnumerable<TableTag>? From { get; set; }

    /// <summary>
    /// Builds a SQL query from the current builder state.
    /// </summary>
    /// <returns>A <see cref="SqlQuery"/> generated from the current builder state.</returns>
    public SqlQuery AsSqlQuery() =>
        SqlGenerator.Update(this);
}

/// <summary>
/// Builds UPDATE query options for the specified model type.
/// </summary>
/// <typeparam name="T">The model type being updated.</typeparam>
public sealed record UpdateBuilder<T> : QueryBuilders.UpdateBuilderBase<T, T>, IQueryBuilder where T : class
{
    /// <summary>
    /// Explains why the one-type PostgreSQL builder cannot accept joins rooted from the target table.
    /// </summary>
    private const string JoinsNotSupportedMessage =
    "PostgreSQL UPDATE joins are rooted from a table in the FROM clause, not from the table being updated. " +
    "This one-type UpdateBuilder<T> exists only for PostgreSQL UPDATE statements that do not use Joins. " +
    "The inherited Joins member must exist because UpdateBuilderBase<T, T> defines it, but using Joins<T> here would incorrectly require the join chain to start with the update type. " +
    "Use UpdateBuilder<T, joinsT> when a PostgreSQL UPDATE statement requires joins.";

    /// <summary>
    /// Generates SQL for the builder state.
    /// </summary>
    private readonly SqlGenerator<T> SqlGenerator = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateBuilder{T, joinsT}"/> class.
    /// </summary>
    /// <param name="encryption">The optional encryption service used for encrypted model properties.</param>
    public UpdateBuilder(IEncryption? encryption = null) =>
        SqlGenerator = new(encryption);

    /// <summary>
    /// Gets or sets the tables to include in the FROM clause.
    /// </summary>
    public IEnumerable<TableTag>? From { get; set; }

    [Obsolete(JoinsNotSupportedMessage, true)]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
    /// <summary>
    /// Gets or sets the obsolete joins member that is intentionally unavailable for this PostgreSQL builder.
    /// </summary>
    public override Joins<T>? Joins { get; set; }
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member


    [Obsolete(JoinsNotSupportedMessage, true)]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
    /// <summary>
    /// Throws at compile time when callers try to add target-rooted joins to this PostgreSQL builder.
    /// </summary>
    /// <param name="joins">The SQL joins used by the query.</param>
    /// <returns>This member is obsolete with <c>error: true</c> and should not be called.</returns>
    public override UpdateBuilder<T> WithJoins(Joins<T>? joins) =>
        this with { Joins = joins };
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member

    /// <summary>
    /// Builds a SQL query from the current builder state.
    /// </summary>
    /// <returns>A <see cref="SqlQuery"/> generated from the current builder state.</returns>
    public SqlQuery AsSqlQuery() =>
        SqlGenerator.Update(this);
}
