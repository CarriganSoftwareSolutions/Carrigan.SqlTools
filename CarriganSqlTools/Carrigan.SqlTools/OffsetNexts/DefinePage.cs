namespace Carrigan.SqlTools.OffsetNexts;

/// <summary>
/// Represents SQL Server’s <c>OFFSET…FETCH NEXT</c> paging feature,
/// expressed through the more tangible concept of a book page.
/// Uses the specified page number and page size to calculate the
/// offset and fetch values required to return the desired page of results.
/// </summary>
/// <remarks>
/// When this paging option is used, an additional <c>ORDER BY</c> criterion for the key
/// property of the queried table is automatically appended at the end of the
/// <c>ORDER BY</c> clause. This ensures stable and consistent results without
/// altering the intended sort order, compensating for quirks in SQL Server’s
/// <c>OFFSET</c> and <c>FETCH NEXT</c> behavior.
/// </remarks>
/// <example>
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
    /// which represents a single results page based on SQL Server’s
    /// <c>OFFSET…FETCH NEXT</c> paging feature.
    /// </summary>
    /// <remarks>
    /// Using this constructor automatically appends an additional ORDER BY criterion
    /// for the key properties of the queried table. This is placed at the end of the
    /// <c>ORDER BY</c> clause to avoid altering the intended sort order, while ensuring
    /// stable and consistent results when using SQL Server’s <c>OFFSET</c> and <c>FETCH NEXT</c>.
    /// </remarks>
    /// <param name="pageNumber">The 1-based page number to retrieve.</param>
    /// <param name="pageSize">The number of rows to include in each page.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="pageNumber"/> or <paramref name="pageSize"/> is less than 1.
    /// </exception>
    public DefinePage(uint pageNumber, uint pageSize)
    {
        if (pageNumber <= 0 || pageSize <= 0)
            throw new ArgumentOutOfRangeException($"{nameof(pageNumber)}, {pageNumber}, and {nameof(pageSize)}, {pageSize} must have a value greater than zero.");

        Offset = (pageNumber - 1) * pageSize;
        Next = pageSize;
    }
}
