namespace Carrigan.SqlTools.OffsetNexts;

/// <summary>
/// This class also represents the Offset Next feature in SQL Server, 
/// however this one wraps the concept with the more tangible idea of a page from a book.
/// The page number and page size are used to calculate the offset and next values in a query needed to result in a given page.
/// </summary>
public class DefinePage : OffsetNext
{
    /// <summary>
    /// The constructor of <see cref="DefinePage"/>
    /// </summary>
    /// <param name="pageNumber">Offset = (pageNumber - 1) * pageSize</param>
    /// <param name="pageSize">Next = pageSize</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public DefinePage(uint pageNumber, uint pageSize)
    {
        if (pageNumber <= 0 || pageSize <= 0)
            throw new ArgumentOutOfRangeException($"{nameof(pageNumber)}, {pageNumber}, and {nameof(pageSize)}, {pageSize} must have a value greater than zero.");

        Offset = (pageNumber - 1) * pageSize;
        Next = pageSize;
    }
}
