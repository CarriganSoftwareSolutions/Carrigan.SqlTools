using Carrigan.SqlTools.Types;
using System.Data;

//TODO: Read Documentation after shifting to base class, Proof read Documentation
namespace Carrigan.SqlTools.Attributes;
/// <summary>
/// This attribute allows overriding the database type for the column associated with the property.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = true)]
public abstract class SqlTypeAttribute : Attribute
{
    /// <summary>
    /// This the Sql Server ADO.Net Type, as well as the text to declare the indicated type in SQL with the supplied sizing arguments.
    /// </summary>
    internal readonly SqlTypeDefinition SqlTypeDefinition;

    protected SqlTypeAttribute(SqlTypeDefinition sqlTypeDefinition) =>
        SqlTypeDefinition = sqlTypeDefinition;
}
