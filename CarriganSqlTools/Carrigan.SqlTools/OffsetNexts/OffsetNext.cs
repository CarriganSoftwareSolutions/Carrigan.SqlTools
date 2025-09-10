namespace Carrigan.SqlTools.OffsetNexts;

/// <summary>
/// This class represents the offset and page feature in SQL Server.  Used together this can define a page of data records.
/// Note: Using this option adds an additional order by criteria for the key fields of the table being queried.
/// This is added to the end of the Order By clause, so as not to affect the order.
/// This is done to ensure a consistent result, due to eccentricities of off set and next in SQL Server/
/// </summary>
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
    /// Represents the SQL Offset
    /// </summary>
    public uint Offset { get; protected set; }
    /// <summary>
    /// Represents the SQL Next
    /// </summary>
    public uint Next { get; protected set; }

    /// <summary>
    /// Protected constructor needed for inheritance.
    /// </summary>
    protected OffsetNext()
    {
    }

    /// <summary>
    /// public constructor to define Offset and Next
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="next"></param>
    public OffsetNext(uint offset, uint next)
    {
        Offset = offset;
        Next = next;
    }

    /// <summary>
    /// Produces the SQL for the given values Offset and Next
    /// </summary>
    /// <returns>the SQL for the given values Offset and Next</returns>
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
