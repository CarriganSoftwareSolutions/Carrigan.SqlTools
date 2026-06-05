using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.Sets;

/// <summary>
/// Represents a SQL Server collection of model properties used as SQL columns.
/// </summary>
/// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
/// <example>
/// <code language="csharp"><![CDATA[
/// using Carrigan.SqlTools.Base.Tests.Helpers;
/// using Carrigan.SqlTools.Base.Tests.TestEntities; //this is where Customer and Order are defined.
/// using Carrigan.SqlTools.Sets;
/// using Carrigan.SqlTools.SqlGenerators;
/// using Carrigan.SqlTools.SqlServer;
/// 
/// ColumnCollection<Customer> columns = new(nameof(Customer.Email));
/// Customer entity = new()
/// {
///     Id = 42,
///     Name = "Hank",
///     Email = "Hank@example.gov"
/// };
/// SqlQuery query = customerGenerator.UpdateById(entity, columns);
/// 
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[ 
/// UPDATE [Customer] 
/// SET [Email] = @Email_1 
/// WHERE [Id] = @Id_2;
/// ]]></code>
/// </example>
/// <example>
/// 
/// <code language="csharp"><![CDATA[
/// using Carrigan.SqlTools.Base.Tests.Helpers;
/// using Carrigan.SqlTools.Base.Tests.TestEntities; //this is where Customer and Order are defined.
/// using Carrigan.SqlTools.Sets;
/// using Carrigan.SqlTools.SqlGenerators;
/// using Carrigan.SqlTools.SqlServer;
/// 
/// //No CollumnCollection
/// Customer entity = new()
/// {
///     Id = 42,
///     Name = "Hank",
///     Email = "Hank@tx.gov",
///     Phone = "+1(555)555-5555"
/// };
/// SqlQuery query = customerGenerator.UpdateById(entity);
/// 
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[ 
/// UPDATE [Customer] 
/// SET [Name] = @Name_1, [Email] = @Email_2, [Phone] = @Phone_3 
/// WHERE [Id] = @Id_4;
/// ]]></code>
/// </example>
public class ColumnCollection<T> : ColumnCollectionBase<T> where T : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnCollection{T}"/> class.
    /// </summary>
    /// <param name="propertyNames">The C# property names representing SQL columns or parameters.</param>
    public ColumnCollection(params IEnumerable<PropertyName> propertyNames) : base(propertyNames)
    {
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnCollection{T}"/> class.
    /// </summary>
    /// <param name="propertyNames">The C# property names representing SQL columns or parameters.</param>
    public ColumnCollection(params IEnumerable<string> propertyNames) : base(propertyNames.Select(propertyName => new PropertyName(propertyName)))
    {
    }

    /// <summary>
    /// Gets the CLR property types supported by this column collection.
    /// </summary>
    protected override HashSet<Type> SupportedTypes =>
        DialectStatics.SupportedTypes;

    /// <summary>
    /// Creates a new column collection from the supplied property names.
    /// </summary>
    /// <param name="propertyNames">The C# property names representing SQL columns or parameters.</param>
    /// <returns>A new column collection containing the supplied property names.</returns>
    protected override ColumnCollection<T> FromPropertyName(IEnumerable<PropertyName> propertyNames) =>
        new (propertyNames);

    /// <summary>
    /// Creates a new column collection with the supplied property added.
    /// </summary>
    /// <param name="propertyName">The C# property name representing the SQL column or parameter.</param>
    /// <returns>A new column collection that includes the supplied property when it is not already present.</returns>
    public override ColumnCollection<T> AppendColumn(PropertyName propertyName) =>
        (ColumnCollection<T>)base.AppendColumn(propertyName);

    /// <summary>
    /// Creates a new column collection with the supplied property added.
    /// </summary>
    /// <param name="propertyName">The C# property name representing the SQL column or parameter.</param>
    /// <returns>A new column collection that includes the supplied property when it is not already present.</returns>
    public override ColumnCollection<T> AppendColumn(string propertyName) =>
        AppendColumn(new PropertyName(propertyName));

    /// <summary>
    /// Creates a new column collection with the supplied properties added.
    /// </summary>
    /// <param name="propertyNames">The C# property names representing SQL columns or parameters.</param>
    /// <returns>A new column collection containing the current properties and the supplied additional properties.</returns>
    public override ColumnCollection<T> ConcatColumn(params IEnumerable<PropertyName> propertyNames) =>
        (ColumnCollection<T>)base.ConcatColumn(propertyNames);

    /// <summary>
    /// Creates a new column collection with the supplied properties added.
    /// </summary>
    /// <param name="propertyNames">The C# property names representing SQL columns or parameters.</param>
    /// <returns>A new column collection containing the current properties and the supplied additional properties.</returns>
    public override ColumnCollection<T> ConcatColumn(params IEnumerable<string> propertyNames) =>
        ConcatColumn(propertyNames.Select(static propertyName => new PropertyName(propertyName)));

}
