using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.SqlGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents the SQL CURRENT_TIMESTAMP function, which returns the current date and time as a timestamp value.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// CurrentTimeStamp currentTimeStamp = new();
/// ColumnBase columnBase = new Column<Customer>(nameof(Customer.Name));
/// SelectBuilder<Customer> selectBuilder = new()
/// {
///     Selects = new SelectTags(new SelectTag(currentTimeStamp, "TimeStamp"), new SelectTag(columnBase, "Name"))
/// };
/// 
/// SqlQuery query = customerGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// --SqlServer
/// SELECT CURRENT_TIMESTAMP() AS [TimeStamp], [Customer].[Name] AS [Name] FROM [Customer]
/// --PostgreSql
/// SELECT CURRENT_TIMESTAMP() AS \"TimeStamp\", \"Customer\".\"Name\" AS \"Name\" FROM \"Customer\"
/// ]]></code>
/// </example>
public class CurrentTimeStamp : FunctionalExpression
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentTimeStamp"/> class.
    /// </summary>
    public CurrentTimeStamp() :base ()
    {
    }

    /// <summary>
    /// Gets the name of the SQL function represented by this expression, which is "CURRENT_TIMESTAMP".
    /// </summary>
    protected override string FunctionName =>
        "CURRENT_TIMESTAMP";
}
