using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using Carrigan.SqlTools.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents the SQL Server GETDATE() function, which returns the current database system timestamp as a datetime value.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// GetDate getDate = new();
/// ColumnBase columnBase = new Column<Customer>(nameof(Customer.Name));
/// SelectBuilder<Customer> selectBuilder = new()
/// {
///     Selects = new SelectTags(new SelectTag(getDate, "Date"), new SelectTag(columnBase, "Name"))
/// };
/// 
/// SqlQuery query = customerGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// --SqlServer
/// SELECT GETDATE() AS [Date], [Customer].[Name] AS [Name] FROM [Customer]
/// ]]></code>
/// </example>
public class GetDate : FunctionalExpression
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetDate"/> class, representing the SQL Server GETDATE() function.
    /// </summary>
    public GetDate() : base()
    { }

    /// <summary>
    /// Gets the name of the SQL function represented by this expression, which is "GETDATE" for SQL Server.
    /// </summary>
    protected override string FunctionName =>
        "GETDATE";
}
