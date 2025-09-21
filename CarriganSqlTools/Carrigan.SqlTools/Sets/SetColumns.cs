using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Sets;
/// <summary>
/// Represents a SQL <c>SET</c> clause used when generating <c>UPDATE</c> statements.
/// Use this class to specify only certain columns to update instead of updating all columns
/// by default. The name mirrors the SQL syntax <c>SET [Column] = @Parameter</c>.
/// </summary>
/// <typeparam name="T">
/// The entity or data model type that defines the table being updated.
/// </typeparam>
/// <para>Update example not using SetColumns</para>
/// <example>
/// <code language="csharp"><![CDATA[
///Customer entity = new()
///{
///    Id = 42,
///    Name = "Hank Hill",
///    Email = "Hank.Hill@example.com",
///    Phone = "+1(555)555-5555"
///};
///SqlQuery query = customerGenerator.UpdateById(entity);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// UPDATE [Customer] SET [Name] = @Name, [Email] = @Email, [Phone] = @Phone 
/// WHERE [Id] = @Id;
/// ]]></code>
/// </example>
/// <example>
/// <para>
/// Update example using SetColumns
/// Note: SetColumns&lt;T&gt; validates the names of the properties, and throws an error if the property isn't valid
/// </para>
/// <code language="csharp"><![CDATA[
/// SetColumns&lt;Customer&gt; columns = new(nameof(Customer.Email));
/// Customer entity = new()
/// {
///    Id = 42,
///    Name = "Hank",
///    Email = "Hank@example.gov"
/// };
/// SqlQuery query = customerGenerator.UpdateById(entity, columns);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// UPDATE [Customer] SET [Email] = @Email 
/// WHERE [Id] = @Id;
/// ]]></code>
/// </example>
public class SetColumns<T> : SqlToolsReflectorCache<T>
{
    /// <summary>
    /// Gets the collection of <see cref="ColumnTag"/> objects representing
    /// the columns used in this instance.
    /// </summary>
    public IEnumerable<ColumnTag> ColumnTags { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SetColumns"/> class,
    /// specifying the properties (columns) to include in the SQL <c>SET</c> clause.
    /// </summary>
    /// <param name="propertyNames">
    /// The names of the properties that represent the column names to be updated.
    /// </param>
    public SetColumns(params IEnumerable<string> propertyNames) =>
        ColumnTags = SqlToolsReflectorCache<T>.GetColumnsFromProperties(propertyNames);

    /// <summary>
    /// Adds an additional column to the <c>SET</c> clause.
    /// </summary>
    /// <param name="propertyName">
    /// The name of the property that represents the column to add.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown if the specified column name is not found.
    /// </exception>
    public void AddColumn(string propertyName)
    {
        ColumnTag? newTag = SqlToolsReflectorCache<T>.GetColumnsFromProperties(propertyName).Single();
        if(newTag is not null)
            ColumnTags = ColumnTags.Append(newTag);
    }
}
