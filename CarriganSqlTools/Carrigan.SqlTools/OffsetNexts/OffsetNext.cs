namespace Carrigan.SqlTools.OffsetNexts;

/// <summary>
/// This class represents the offset and page feature in SQL Server.  Used together this can define a page of data records.
/// </summary>
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
