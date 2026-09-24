using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.SqlGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents the PostgreSQL <c>SPLIT_PART</c> function.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// SplitPart expression = new(new Column<Customer>(nameof(Customer.Name)), " ", 2);
/// SelectBuilder<Customer> selectBuilder = new() { Selects = expression.AsSelectTag("Value") };
/// SqlQuery query = customerGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SECLECT SPLIT_PART("Customer"."Name", $1, $2) AS "Value" FROM "Customer"
/// ]]></code>
/// </example>
public class SplitPart : FunctionalExpression
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SplitPart"/> class.
    /// </summary>
    /// <param name="stringValue">
    /// The string value to split.
    /// </param>
    /// <param name="stringDelimiter">
    /// The delimiter to use for splitting.
    /// </param>
    /// <param name="intPosition">
    /// The position of the part to retrieve.
    /// </param>
    public SplitPart(SqlExpression stringValue, SqlExpression stringDelimiter, SqlExpression intPosition) :
        base([ValidateValue(stringValue), ValidateValue(stringDelimiter), ValidateValue(intPosition)])
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SplitPart"/> class.
    /// </summary>
    /// <param name="stringValue">
    /// The string value to split.
    /// </param>
    /// <param name="stringDelimiter">
    /// The delimiter to use for splitting.
    /// </param>
    /// <param name="intPosition">
    /// The position of the part to retrieve.
    /// </param>
    public SplitPart(SqlExpression stringValue, string stringDelimiter, int intPosition) :
        base([ValidateValue(stringValue), ValidateParameterValue(stringDelimiter), ValidateParameterValue(intPosition)])
    { }

    /// <summary>
    /// Gets the name of the function.
    /// </summary>
    protected override string FunctionName => "SPLIT_PART";

}
