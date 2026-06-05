
using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.SqlServer;

/// <summary>
/// Builds INSERT query options for a specific entity type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">
/// The entity/model type that defines the table into which data will be inserted.
/// </typeparam>
/// <example>
/// <code language="csharp"><![CDATA[
/// IEnumerable<Customer> customers =
///     [
///         new()
///         {
///             Id = 42,
///             Name = "Hank",
///             Email = "Hank@example.com",
///             Phone = "+1(555)555-5555"
///         },
///         new()
///         {
///             Id = 732,
///             Name = "Homer",
///             Email = "Homer@example.com",
///             Phone = "+1(555)555-1234"
///         },
///     ];
/// InsertBuilder<Customer> insertBuilder = new()
/// {
///     Records = customers
/// };
/// 
/// SqlQuery query = customerGenerator.Insert(insertBuilder);
/// ]]></code>
///
/// <para>Resulting SQL:</para>
///
/// <code><![CDATA[
/// INSERT INTO [Customer] ([Id], [Name], [Email], [Phone]) 
/// VALUES (@Id_1, @Name_2, @Email_3, @Phone_4), (@Id_5, @Name_6, @Email_7, @Phone_8);
/// ]]></code>
/// </example>
public sealed record InsertBuilder<T> : QueryBuilders.InsertBuilderBase<T>, IQueryBuilder where T : class
{
    /// <summary>
    /// Generates SQL for the builder state.
    /// </summary>
    private readonly SqlGenerator<T> SqlGenerator = new();
    /// <summary>
    /// Initializes a new instance of the <see cref="InsertBuilder{T}"/> class.
    /// </summary>
    /// <param name="encryption">The optional encryption service used for encrypted model properties.</param>
    public InsertBuilder(IEncryption? encryption = null) =>
        SqlGenerator = new(encryption);
    /// <summary>
    /// Builds a SQL query from the current builder state.
    /// </summary>
    /// <returns>A <see cref="SqlQuery"/> generated from the current builder state.</returns>
    public SqlQuery AsSqlQuery() =>
        SqlGenerator.Insert(this);
}
