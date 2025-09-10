using Carrigan.SqlTools.OrderByItems;
using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.OffsetNexts;

/// <summary>
/// This class also represents the Offset Next feature in SQL Server, 
/// however this one wraps the concept with the more tangible idea of a page from a book.
/// The page number and page size are used to calculate the offset and next values in a query needed to result in a given page.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// DefinePage definePage = new(2, 25);
/// SqlQuery query = customerGenerator.Select(null, null, null, definePage);
/// 
/// // SELECT [Customer].* FROM [Customer] 
/// // ORDER BY [Customer].[Id] ASC 
/// // OFFSET 25 ROWS FETCH NEXT 25 ROWS ONLY
/// 
/// DefinePage definePage = new(2, 25);
/// OrderByItem&lt;Customer&gt; orderBy = new(nameof(Customer.Name));
/// SqlQuery query = customerGenerator.Select(null, null, orderBy, definePage);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* FROM [Customer] 
/// ORDER BY [Customer].[Name] ASC, [Customer].[Id] 
/// ASC OFFSET 25 ROWS FETCH NEXT 25 ROWS ONLY
/// ]]></code>
/// </example>
public class DefinePage : OffsetNext
{
    /// <summary>
    /// The constructor of <see cref="DefinePage"/>
    /// Note: Using this option adds an additional order by criteria for the key fields of the table being queried.
    /// This is added to the end of the Order By clause, so as not to affect the order.
    /// This is done to ensure a consistent result, due to eccentricities of off set and next in SQL Server.
    /// Offset = (pageNumber - 1) * pageSize
    /// Next = pageSize
    /// </summary>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public DefinePage(uint pageNumber, uint pageSize)
    {
        if (pageNumber <= 0 || pageSize <= 0)
            throw new ArgumentOutOfRangeException($"{nameof(pageNumber)}, {pageNumber}, and {nameof(pageSize)}, {pageSize} must have a value greater than zero.");

        Offset = (pageNumber - 1) * pageSize;
        Next = pageSize;
    }
}
