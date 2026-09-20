using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Provides extension methods for the SqlExpression class.
/// </summary>
public static class SqlExpressionExtensions
{
    /// <summary>
    /// Creates a SelectTag from a SqlExpression and an AliasName.
    /// </summary>
    /// <param name="sqlExpression">
    /// The SqlExpression to be converted into a SelectTag.
    /// </param>
    /// <param name="aliasName">
    /// The AliasName to be associated with the SelectTag.
    /// </param>
    /// <returns>
    /// A SelectTag that represents the given SqlExpression with the specified AliasName.
    /// </returns>
    public static SelectTag AsSelectTag(this SqlExpression sqlExpression, AliasName aliasName) =>
        new(sqlExpression, aliasName);

    /// <summary>
    /// Creates a SelectTag from a SqlExpression and a string alias name.
    /// </summary>
    /// <param name="sqlExpression">
    /// The SqlExpression to be converted into a SelectTag.
    /// </param>
    /// <param name="aliasName">
    /// The string alias name to be associated with the SelectTag.
    /// </param>
    /// <returns>
    /// A SelectTag that represents the given SqlExpression with the specified alias name.
    /// </returns>
    [ExternalOnly]
    public static SelectTag AsSelectTag(this SqlExpression sqlExpression, string aliasName) =>
        new(sqlExpression, new AliasName(aliasName));
}
