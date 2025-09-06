namespace Carrigan.SqlTools.OffsetNexts;

public class DefinePage
{
    public uint Offset { get; private set; }
    public uint Next { get; private set; }
    public  DefinePage(uint pageNumber, uint pageSize)
    {
        if (pageNumber <= 0 || pageSize <= 0)
            throw new ArgumentOutOfRangeException($"{nameof(pageNumber)}, {pageNumber}, and {nameof(pageSize)}, {pageSize} must have a value greater than zero.");

        Offset = (pageNumber - 1) * pageSize;
        Next = pageSize;
    }
}
