using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.SqlGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents the SQL function CURRENT_DATE, which returns the current date in SQL.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// CurrentDate currentDate = new();
/// ColumnBase columnBase = new Column<Customer>(nameof(Customer.Name));
/// SelectBuilder<Customer> selectBuilder = new()
/// {
///     Selects = new SelectTags(new SelectTag(currentDate, "Date"), new SelectTag(columnBase, "Name"))
/// };
/// 
/// SqlQuery query = customerGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// --SqlServer
/// SELECT CURRENT_DATE AS [Date], [Customer].[Name] AS [Name] FROM [Customer]
/// --PostgreSql
/// SELECT CURRENT_DATE AS \"Date\", \"Customer\".\"Name\" AS \"Name\" FROM \"Customer\"
/// ]]></code>
/// </example>
public class CurrentDate : FunctionalExpression
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentDate"/> class, representing the SQL function CURRENT_DATE.
    /// </summary>
    public CurrentDate() : base()
    {}

    /// <summary>
    /// Gets the name of the SQL function represented by this expression, which is "CURRENT_DATE".
    /// </summary>
    protected override string FunctionName => 
        "CURRENT_DATE";

    /// <summary>
    /// Gets a value indicating whether parentheses are rendered after CURRENT_DATE.
    /// </summary>
    protected override bool RenderParentheses =>
        false;
}
