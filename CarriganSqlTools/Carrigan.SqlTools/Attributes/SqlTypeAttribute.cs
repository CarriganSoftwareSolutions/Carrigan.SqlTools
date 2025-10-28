using Carrigan.SqlTools.Types;
using System.Data;
using System.Drawing;

//TODO: Proof read Documentation
namespace Carrigan.SqlTools.Attributes;
[AttributeUsage(AttributeTargets.Property)]
/// <summary>
/// This attribute allows overriding the database type for the column associated with the property.
/// </summary>
public class SqlTypeAttribute : Attribute
{
    /// <summary>
    /// This the Sql Server ADO.Net Type, as well as the text to declare the indicated type in SQL with the supplied sizing arguments.
    /// </summary>
    internal readonly SqlTypeDefinition SqlTypeDefinition;
    //TODO: bullet proofing.

    /// <summary>
    /// Attribute constructor to use when the type has no sizing arguments, or the default size is acceptable.
    /// </summary>
    /// <param name="type">The Sql Server ADO.Net Type</param>
    public SqlTypeAttribute(SqlDbType type) =>
        SqlTypeDefinition = new SqlTypeDefinition(type);

    /// <summary>
    /// Attribute constructor to use when specifying Size, Precision or Scale.
    /// </summary>
    /// <param name="type">The Sql Server ADO.Net Type</param>
    /// <param name="size">
    /// Parameter to use to specify a size argument.
    /// SQL Types with Size:
    /// Binary (1 to 8000)
    /// Char (1 to 8000)
    /// NChar (1 to 4000)
    /// NVarChar (1 to 4000)
    /// VarBinary (1 to 8000)
    /// VarChar (1 to 8000)
    /// </param>
    /// <param name="precision">
    /// Parameter to use to specify a precision argument.
    /// SQL Types with Precision:
    /// Float (1 to 53)
    /// Decimal (1 to 38)
    /// Note: For Decimal the precision + scale  cannot be greater than 38
    /// </param>
    /// <param name="scale">
    /// Parameter to use to specify a scale argument.
    /// SQL Types with Scale:
    /// Time (0 to 7)
    /// DateTime2 (0 to 7)
    /// DateTimeOffset (0 to 7)
    /// Decimal (0 to 38)
    /// Note: For Decimal the precision + scale  cannot be greater than 38
    /// </param>
    public SqlTypeAttribute(SqlDbType type, int size = -1, byte precision = byte.MaxValue, byte scale = byte.MaxValue) =>
        SqlTypeDefinition = new SqlTypeDefinition(type, (size == -1 ? null : size), (precision == byte.MaxValue ? null : precision), (scale == byte.MaxValue ? null : scale));

    /// <summary>
    /// Attribute constructor to use when you wish to specify the use of the MAX sizing argument.
    /// For use with VARCHAR, NVARCHAR and VARBINARY.
    /// Though if you set max to false, you can safely use it with other types.
    /// </summary>
    /// <param name="type">The Sql Server ADO.Net Type</param>
    /// <param name="useMax">
    /// Attribute constructor to use when you wish to specify the use of the MAX sizing argument.
    /// For use with VARCHAR, NVARCHAR and VARBINARY.
    /// Though if you set max to false, you can safely use it with other types.
    /// </param>
    public SqlTypeAttribute(SqlDbType type, bool useMax) =>
        SqlTypeDefinition = new SqlTypeDefinition(type, useMax);
}
