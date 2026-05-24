namespace Carrigan.SqlTools.Paging;

/// <summary>
/// Represents SQL Server’s <c>OFFSET … FETCH NEXT</c> paging feature,
/// defining the row offset and fetch count used to return a specific range of results.
/// </summary>
/// <remarks>
/// <para>
/// This class only defines paging behavior and does not modify SQL directly.
/// When an <see cref="OffsetFetchNext"/> (or a derived type such as <see cref="DefinePage"/>) is passed to the SQL generator,
/// the generator automatically appends an additional <c>ORDER BY</c> criterion for key columns (if not already present)
/// to ensure deterministic paging results.
/// </para>
/// <para>
/// This compensates for SQL Server’s requirement that <c>OFFSET</c> and <c>FETCH NEXT</c> be used in conjunction with an
/// <c>ORDER BY</c> clause.
/// </para>
/// </remarks>
/// <example>
/// <code language="csharp"><![CDATA[
/// OffsetNext offsetNext = new(50, 25);
/// SqlQuery query = customerGenerator.Select(null, null, null, null, offsetNext);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].*
/// FROM [Customer]
/// ORDER BY [Customer].[Id] ASC
/// OFFSET 50 ROWS FETCH NEXT 25 ROWS ONLY
/// ]]></code>
/// </example>
/// 
/// <example>
/// <code language="csharp"><![CDATA[
/// OffsetNext offsetNext = new(50, 25);
/// OrderBy<Customer> orderBy = new(nameof(Customer.Name));
/// SqlQuery query = customerGenerator.Select(null, null, null, orderBy, offsetNext);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].*
/// FROM [Customer]
/// ORDER BY [Customer].[Name] ASC, [Customer].[Id] ASC
/// OFFSET 50 ROWS FETCH NEXT 25 ROWS ONLY
/// ]]></code>
/// </example>
public class OffsetFetchNext : PagingBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OffsetFetchNext"/> class,
    /// defining both the SQL <c>OFFSET</c> and <c>FETCH NEXT</c> values.
    /// </summary>
    /// <param name="offset">The number of rows to skip before starting to return results.</param>
    /// <param name="next">The number of rows to return after the offset.</param>
    public OffsetFetchNext(uint offset, uint next) : base(offset, next)
    { }
}
