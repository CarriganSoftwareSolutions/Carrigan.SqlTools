using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using System.Linq;

namespace Carrigan.SqlTools.Sets;
/// <summary>
/// This class is used as part of generating SQL updates statements. 
/// It used when you need to change from the default behavior to update all columns to only update specific columns.
/// This class is named to mirror the SQL "SET [Column] = @Parameter".
/// </summary>
/// <typeparam name="T"></typeparam>
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
    /// Name of the columns used in the instance
    /// </summary>
    public IEnumerable<ColumnTag> ColumnTags { get; private set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="propertyNames">Names of the properties that represent the names of columns to be used</param>
    public SetColumns(params IEnumerable<string> propertyNames)
    {
        SqlToolsReflectorCache<T>.ValidateEntityPropertyNames(propertyNames);
        ColumnTags = propertyNames.Select(property => GetColumnTagByProperty(property)).OfType<ColumnTag>();
    }

    /// <summary>
    /// Add an additional column
    /// </summary>
    /// <param name="propertyName">additional property that represent the names of a column to be used</param>
    /// <exception cref="ArgumentException">Column name not found.</exception>
    public void AddColumn(string propertyName)
    {
        ColumnTag? newTag = GetColumnTagByProperty(propertyName);
        if(newTag is not null)
            ColumnTags = ColumnTags.Append(newTag);
    }
}
