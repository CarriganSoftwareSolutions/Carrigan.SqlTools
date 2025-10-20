namespace Carrigan.SqlTools.OffsetNexts;

/// <summary>
/// Represents SQL Server’s <c>OFFSET … FETCH NEXT</c> paging feature,
/// expressed through the tangible concept of a book page.
/// Uses the specified page number and page size to calculate the
/// offset and fetch values required to return the desired page of results.
/// </summary>
/// <remarks>
/// This class only defines paging behavior (page size and page number).  
/// When a <see cref="DefinePage"/> instance is passed to the SQL generator,
/// the generator automatically appends an additional <c>ORDER BY</c> criterion
/// for key columns (if not already present) to ensure stable paging results.
/// </remarks>
/// <code language="csharp"><![CDATA[
/// DefinePage definePage = new(2, 25);
/// SqlQuery query = customerGenerator.Select(null, null, null, null, definePage);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* 
/// FROM [Customer] 
/// ORDER BY [Customer].[Id] ASC 
/// OFFSET 25 ROWS FETCH NEXT 25 ROWS ONLY
/// ]]></code>
/// </example>
/// <example>
/// <code language="csharp"><![CDATA[
/// DefinePage definePage = new(2, 25);
/// OrderByItem<Customer> orderBy = new(nameof(Customer.Name));
/// SqlQuery query = customerGenerator.Select(null, null, null, orderBy, definePage);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* 
/// FROM [Customer] 
/// ORDER BY [Customer].[Name] ASC, [Customer].[Id] ASC 
/// OFFSET 25 ROWS FETCH NEXT 25 ROWS ONLY
/// ]]></code>
/// </example>
public class DefinePage : OffsetNext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefinePage"/> class,
    /// representing a single results page based on SQL Server’s
    /// <c>OFFSET … FETCH NEXT</c> paging feature.
    /// </summary>
    /// <remarks>
    /// This class does not modify or generate SQL directly.  
    /// When a <see cref="DefinePage"/> instance is passed to the SQL generator,
    /// the generator automatically appends an additional <c>ORDER BY</c> criterion
    /// for key columns (if not already present) to ensure stable paging results.
    /// </remarks>
    /// <param name="pageNumber">The 1-based page number to retrieve.</param>
    /// <param name="pageSize">The number of rows to include in each page.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when either <paramref name="pageNumber"/> or <paramref name="pageSize"/> is less than 1.
    /// </exception>
    public DefinePage(uint pageNumber, uint pageSize)
    {
        if (pageNumber <= 0 || pageSize <= 0)
            throw new ArgumentOutOfRangeException($"{nameof(pageNumber)}, {pageNumber}, and {nameof(pageSize)}, {pageSize} must have a value greater than zero.");

        Offset = (pageNumber - 1) * pageSize;
        Next = pageSize;
    }
}
