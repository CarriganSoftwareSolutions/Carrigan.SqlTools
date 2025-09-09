namespace Carrigan.SqlTools.OffsetNexts;

public class OffsetNext
{
    public uint Offset { get; protected set; }
    public uint Next { get; protected set; }

    protected OffsetNext()
    {
    }

    public OffsetNext(uint offset, uint next)
    {
        Offset = offset;
        Next = next;
    }

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
