namespace Carrigan.SqlTools.OffsetNexts;

/// <summary>
/// Represents SQL Server’s <c>OFFSET … FETCH NEXT</c> paging feature,  
/// defining the row offset and fetch count used to return a specific range of results.
/// </summary>
/// <remarks>
/// This class only defines paging behavior and does not modify SQL directly.  
/// When a <see cref="OffsetNext"/> (or derived class such as <see cref="DefinePage"/>) 
/// is passed to the SQL generator, the generator automatically appends an additional
/// <c>ORDER BY</c> criterion for key columns (if not already present) to ensure
/// deterministic paging results.  
/// This compensates for SQL Server’s requirement that <c>OFFSET</c> and <c>FETCH NEXT</c>
/// be used in conjunction with an <c>ORDER BY</c> clause.
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
/// <example>
/// <code language="csharp"><![CDATA[
/// OffsetNext offsetNext = new(50, 25);
/// OrderByItem<Customer> orderBy = new(nameof(Customer.Name));
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
public class OffsetNext
{
    /// <summary>
    /// Gets or sets the SQL <c>OFFSET</c> value, representing the number of rows to skip before returning results.
    /// </summary>
    public uint Offset { get; protected set; }

    /// <summary>
    /// Gets or sets the SQL <c>FETCH NEXT</c> value, representing the number of rows to return after the offset.
    /// </summary>
    public uint Next { get; protected set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OffsetNext"/> class
    /// for use by derived types.
    /// </summary>

    protected OffsetNext()
    {
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="OffsetNext"/> class,
    /// defining both the SQL <c>OFFSET</c> and <c>FETCH NEXT</c> values.
    /// </summary>
    /// <param name="offset">The number of rows to skip before starting to return results.</param>
    /// <param name="next">The number of rows to return after the offset.</param>
    public OffsetNext(uint offset, uint next)
    {
        Offset = offset;
        Next = next;
    }

    /// <summary>
    /// Generates the SQL fragment representing the <c>OFFSET</c> and <c>FETCH NEXT</c> clauses.
    /// </summary>
    /// <returns>
    /// A SQL string containing the <c>OFFSET</c> and <c>FETCH NEXT</c> clauses,  
    /// or an empty string if both <see cref="Offset"/> and <see cref="Next"/> are zero.
    /// </returns>
    internal string ToSql()
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
