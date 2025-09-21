namespace Carrigan.SqlTools.OffsetNexts;

/// <summary>
/// Represents SQL Server’s <c>OFFSET…FETCH NEXT</c> paging feature, which can be
/// used to define a page of data records.
/// </summary>
/// <remarks>
/// When this paging option is used, an additional <c>ORDER BY</c> criterion for the key
/// fields of the queried table is automatically appended at the end of the
/// <c>ORDER BY</c> clause. This ensures stable and consistent results without
/// altering the intended sort order, compensating for quirks in SQL Server’s
/// <c>OFFSET</c> and <c>FETCH NEXT</c> behavior.
/// </remarks>
/// <example>
/// <code language="csharp"><![CDATA[
/// OffsetNext offsetNext = new(50, 25);
/// SqlQuery query = customerGenerator.Select(null, null, null, offsetNext);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* FROM [Customer] 
/// ORDER BY [Customer].[Id] ASC 
/// OFFSET 50 ROWS FETCH NEXT 25 ROWS ONLY
/// ]]></code>
/// </example>
public class OffsetNext
{
    /// <summary>
    /// Represents the SQL <c>OFFSET</c> clause used for result set paging.
    /// </summary>
    public uint Offset { get; protected set; }
    /// <summary>
    /// Represents the SQL <c>FETCH NEXT</c> clause used for result set paging.
    /// </summary>
    public uint Next { get; protected set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OffsetNext"/> class
    /// for use by derived classes.
    /// </summary>

    protected OffsetNext()
    {
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="OffsetNext"/> class,
    /// defining the SQL <c>OFFSET</c> and <c>FETCH NEXT</c> values for paging.
    /// </summary>
    /// <param name="offset">The number of rows to skip before starting to return rows.</param>
    /// <param name="next">The number of rows to return after the offset.</param>
    public OffsetNext(uint offset, uint next)
    {
        Offset = offset;
        Next = next;
    }

    /// <summary>
    /// Generates the SQL fragment for the specified <c>OFFSET</c> and <c>FETCH NEXT</c> values.
    /// </summary>
    /// <returns>
    /// A SQL string representing the <c>OFFSET</c> and <c>FETCH NEXT</c> clauses.
    /// </returns>
    public string ToSql()
    {
        if(Next == 0 && Offset ==0)
        {
            return string.Empty;
        }
        else if(Next == 0)
        {
            return $"OFFSET {Offset}";
        }
        else
        {
            return $"OFFSET {Offset} ROWS FETCH NEXT {Next} ROWS ONLY";
        }
    }
}
