namespace Carrigan.SqlTools.Paging;

/// <summary>
/// Represents the base class for paging strategies, defining common properties for row offset and fetch next count aka limit and offset.
/// </summary>
public abstract class PagingBase
{
    /// <summary>
    /// Gets the number of rows to skip before starting to return results. This value is used as the OFFSET in SQL paging clauses.
    /// </summary>
    public uint Offset { get; protected set; }
    
    /// <summary>
    /// Gets the number of rows to return after the offset. This value is used as the FETCH NEXT or LIMIT in SQL paging clauses.
    /// </summary>
    public uint Next { get; protected set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PagingBase"/> class with default values for Offset and Next.
    /// </summary>
    protected PagingBase() 
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PagingBase"/> class with specified values for Offset and Next.
    /// </summary>
    /// <param name="offset">The number of rows to skip before starting to return results.</param>
    /// <param name="next">The number of rows to return after the offset (fetch next aka limit).</param>
    protected PagingBase(uint offset, uint next)
    {
        Offset = offset;
        Next = next;
    }
}
