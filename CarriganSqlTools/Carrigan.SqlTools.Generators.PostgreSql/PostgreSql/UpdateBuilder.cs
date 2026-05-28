using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.PostgreSql;

/// <summary>
/// Represents the options used to build an UPDATE query for the specified model type.
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

    private readonly SqlGenerator<T> SqlGenerator = new();

    /// <summary>
    /// Gets or sets the tables to include in the FROM clause.
    /// </summary>
    /// <typeparam name="T">The model type being updated.</typeparam>
    /// <typeparam name="joinsT">The model type used as the starting point for the join collection. This type should represent one of the source tables in the FROM clause.</typeparam>
    /// <remarks>
    /// For PostgreSQL, <typeparamref name="joinsT" /> should represent one of the source tables in the UPDATE FROM clause.
    /// </remarks>
    public IEnumerable<TableTag>? From { get; set; }

    public SqlQuery AsSqlQuery() =>
        SqlGenerator.Update(this);
}

/// <summary>
/// Represents the options used to build an UPDATE query for the specified model type.
/// </summary>
/// <typeparam name="T">The model type being updated.</typeparam>
public sealed record UpdateBuilder<T> : QueryBuilders.UpdateBuilderBase<T, T>, IQueryBuilder where T : class
{
    private const string JoinsNotSupportedMessage =
    "PostgreSQL UPDATE joins are rooted from a table in the FROM clause, not from the table being updated. " +
    "This one-type UpdateBuilder<T> exists only for PostgreSQL UPDATE statements that do not use Joins. " +
    "The inherited Joins member must exist because UpdateBuilderBase<T, T> defines it, but using Joins<T> here would incorrectly require the join chain to start with the update type. " +
    "Use UpdateBuilder<T, joinsT> when a PostgreSQL UPDATE statement requires joins.";

    private readonly SqlGenerator<T> SqlGenerator = new();

    /// <summary>
    /// Gets or sets the tables to include in the FROM clause.
    /// </summary>
    public IEnumerable<TableTag>? From { get; set; }

    [Obsolete(JoinsNotSupportedMessage, true)]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
    public override Joins<T>? Joins { get; set; }
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member


    [Obsolete(JoinsNotSupportedMessage, true)]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
    public override UpdateBuilder<T> WithJoins(Joins<T>? joins) =>
        this with { Joins = joins };
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member

    public SqlQuery AsSqlQuery() =>
        SqlGenerator.Update(this);
}
