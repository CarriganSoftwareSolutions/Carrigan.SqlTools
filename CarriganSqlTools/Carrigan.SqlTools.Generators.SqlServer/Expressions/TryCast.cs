using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using Carrigan.SqlTools.Types;
using System.Linq.Expressions;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents the SQL Server <c>TRY_CAST</c> expression, which attempts to convert an expression to another SQL type.
/// </summary>
/// <remarks>
/// SQL Server returns <c>NULL</c> when an ordinary conversion fails. Conversions that SQL Server explicitly disallows
/// can still raise an error. <c>TRY_CAST</c> is available in SQL Server 2012 (11.x) and later.
/// </remarks>
/// <example>
/// <code language="csharp"><![CDATA[
/// TryCast expression = new(new Parameter("123"), SqlServerTypesProvider.AsInt(true));
/// 
/// SelectBuilder<Customer> selectBuilder = new()
/// {
///     Selects = expression.AsSelectTag("Value")
/// };
/// 
/// SqlQuery query = customerGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// --SqlServer
/// SELECT TRY_CAST(@Parameter_1 AS INT) AS [Value] FROM [Customer]
/// ]]></code>
/// </example>
public class TryCast : Cast
{
    /// <summary>
    /// Initializes a <c>TRY_CAST(expression AS type)</c> expression.
    /// </summary>
    /// <param name="sqlExpression">The expression to attempt to convert.</param>
    /// <param name="fieldProperties">The SQL Server field properties describing the target type.</param>
    public TryCast(SqlExpression sqlExpression, FieldProperties fieldProperties) :
        base(ValidateValue(sqlExpression), fieldProperties ?? throw new ArgumentNullException(nameof(fieldProperties)))
    {
    }

    /// <summary>
    /// Generates SQL Server <c>TRY_CAST(expression AS type)</c> fragments.
    /// </summary>
    /// <param name="dialect">The SQL dialect used to render the target type.</param>
    /// <returns>The SQL fragments for the expression.</returns>
    public override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect)
    {
        yield return new SqlFragmentText("TRY_CAST(");

        foreach (ISqlFragment sqlFragment in SqlExpression.ToSqlFragments(dialect))
            yield return sqlFragment;

        yield return new SqlFragmentText($" AS {dialect.RenderCastType(FieldProperties)})");
    }
}
