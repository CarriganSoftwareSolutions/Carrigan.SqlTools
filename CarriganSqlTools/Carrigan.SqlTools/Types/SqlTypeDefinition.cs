using Carrigan.SqlTools.Exceptions;
using System.Data;

//IGNORE SPELLING: xml, unicode
//TODO:  documentation , unit tests
namespace Carrigan.SqlTools.Types;

/// <summary>
/// Extensions that map <see cref="SqlDbType"/> to SQL Server type keywords,
/// optionally validating and applying precision/scale (or length/MAX where applicable).
/// </summary>
public class SqlTypeDefinition
{
    /// <summary>
    /// The Sql Server ADO.Net Type
    /// </summary>
    public SqlDbType Type { get; init; }

    //TODO: Documentation, 
    public int? Size { get; init; } = null;

    //TODO: Documentation,
    public byte? Precision { get; init; } = null;

    /// <summary>
    /// For DECIMAL/NUMERIC: scale (digits to right of the decimal).
    /// For TIME/DATETIME2/DATETIMEOFFSET: fractional seconds precision (0–7), as per ADO.NET SqlParameter.Scale.
    /// </summary>
    public byte? Scale { get; init; } = null;

    public bool UseMax { get; init; } = false;

    /// <summary>
    /// This represents the text to declare the indicated type in SQL with the supplied sizing arguments.
    /// </summary>
    public string TypeDeclaration { get; init; } = string.Empty;


    private SqlTypeDefinition()
    {
    }

    #region helper methods
    private static void EnsureRange(SqlDbType type, string name, int value, int min, int max)
    {
        if (value < min || value > max)
            throw new SqlTypeArgumentOutOfRangeException(type, name, value, min, max);
    }

    private static SqlTypeDefinition WithSize(SqlDbType type, int? size, int min, int max)
    {
        if (size is not null) EnsureRange(type, "size", size.Value, min, max);
        return new() { Type = type, Size = size, TypeDeclaration = size is null ? ToSql(type) : $"{ToSql(type)}({size})" };
    }

    private static SqlTypeDefinition ByType(SqlDbType type) => new ()
    {
        Type = type,
        TypeDeclaration = ToSql(type)
    };
    private static SqlTypeDefinition AsMax(SqlDbType type) => new()
    {
        Type = type,
        TypeDeclaration = $"{ToSql(type)}(MAX)"
    };
    #endregion

    #region UniqueIdentifier
    public static SqlTypeDefinition AsUniqueIdentifier() =>
        ByType(SqlDbType.UniqueIdentifier);
    #endregion

    #region Chars

    public static SqlTypeDefinition AsChar(int? size = null) =>
        WithSize(SqlDbType.Char, size, 1, 8000);

    public static SqlTypeDefinition AsNChar(int? size = null) =>
        WithSize(SqlDbType.NChar, size, 1, 4000);

    public static SqlTypeDefinition AsVarChar(int? size = null) =>
        WithSize(SqlDbType.VarChar, size, 1, 8000);

    public static SqlTypeDefinition AsNVarChar(int? size = null) =>
        WithSize(SqlDbType.NVarChar, size, 1, 4000);

    public static SqlTypeDefinition AsVarCharMax() =>
        AsMax(SqlDbType.VarChar);

    public static SqlTypeDefinition AsNVarCharMax() =>
        AsMax(SqlDbType.NVarChar);

    public static SqlTypeDefinition AsText() =>
        ByType(SqlDbType.Text);

    public static SqlTypeDefinition AsNText() =>
        ByType(SqlDbType.NText);

    #endregion

    #region Binary

    public static SqlTypeDefinition AsBinary(int? size = null) =>
        WithSize(SqlDbType.Binary, size, 1, 8000);

    public static SqlTypeDefinition AsVarBinary(int? size = null) =>
        WithSize(SqlDbType.VarBinary, size, 1, 8000);

    public static SqlTypeDefinition AsVarBinaryMax() =>
        AsMax(SqlDbType.VarBinary);

    public static SqlTypeDefinition AsImage() =>
        ByType(SqlDbType.Image);
    #endregion

    #region Bit

    public static SqlTypeDefinition AsBit() =>
        ByType(SqlDbType.Bit);
    #endregion

    #region Integers
    public static SqlTypeDefinition AsTinyInt() =>
        ByType(SqlDbType.TinyInt);

    public static SqlTypeDefinition AsSmallInt() =>
        ByType(SqlDbType.SmallInt);

    public static SqlTypeDefinition AsInt() =>
        ByType(SqlDbType.Int);

    public static SqlTypeDefinition AsBigInt() =>
        ByType(SqlDbType.BigInt);
    #endregion

    #region Floating Point
    public static SqlTypeDefinition AsReal() =>
        ByType(SqlDbType.Real);

    public static SqlTypeDefinition AsFloat(byte? precision = null)
    {
        SqlDbType type = SqlDbType.Float;
        if (precision is not null && (precision < 1 || precision > 53))
            throw new SqlTypeArgumentOutOfRangeException(type, "precision", precision.Value, 1, 53);
        return new SqlTypeDefinition()
        {
            Type = type,
            Precision = precision,
            TypeDeclaration = precision is not null ? $"{ToSql(type)}({precision})" : ToSql(type)
        };
    }
    #endregion

    #region decimal point
    public static SqlTypeDefinition AsDecimal() =>
        ByType(SqlDbType.Decimal);

    public static SqlTypeDefinition AsDecimal(byte precision)
    {
        SqlDbType type = SqlDbType.Decimal;

        EnsureRange(SqlDbType.Decimal, "precision", precision, 1, 38);

        return new SqlTypeDefinition()
        {
            Type = type,
            Precision = precision,
            TypeDeclaration = $"{ToSql(type)}({precision})"
        };
    }

    public static SqlTypeDefinition AsDecimal(byte precision, byte scale)
    {
        SqlDbType type = SqlDbType.Decimal;

        EnsureRange(SqlDbType.Decimal, "precision", precision, 1, 38);

        EnsureRange(SqlDbType.Decimal, "precision", scale, 0, precision);

        return new SqlTypeDefinition()
        {
            Type = type,
            Precision = precision,
            Scale = scale,
            TypeDeclaration = $"{ToSql(type)}({precision}, {scale})"
        };
    }

    public static SqlTypeDefinition AsMoney() =>
        ByType(SqlDbType.Money);
    public static SqlTypeDefinition AsSmallMoney() =>
        ByType(SqlDbType.SmallMoney);
    #endregion

    #region DateTime

    public static SqlTypeDefinition AsDateTime2(byte? fractionalSecondPrecision = null)
    {
        SqlDbType type = SqlDbType.DateTime2;
        if (fractionalSecondPrecision is not null)
        {
            if (fractionalSecondPrecision < 0 || fractionalSecondPrecision > 7)
                throw new SqlTypeArgumentOutOfRangeException(type, "fractionalSecondPrecision", fractionalSecondPrecision.Value, 0, 7);
        }
        return new()
        {
            Type = type,
            Scale = fractionalSecondPrecision, //From what I can tell, in sql server it is called precision, but in ADO.Net you set scale.
            TypeDeclaration = fractionalSecondPrecision is not null ? $"{ToSql(type)}({fractionalSecondPrecision})" : ToSql(type)
        };
    }

    public static SqlTypeDefinition AsDateTimeOffset(byte? precision = null)
    {
        SqlDbType type = SqlDbType.DateTimeOffset;
        if (precision is not null)
        {
            if (precision < 0 || precision > 7)
                throw new SqlTypeArgumentOutOfRangeException(type, "precision", precision.Value, 0, 7);
        }
        return new()
        {
            Type = type,
            Scale = precision, //From what I can tell, in sql server it is called precision, but in ADO.Net you set scale.
            TypeDeclaration = precision is not null ? $"{ToSql(type)}({precision})" : ToSql(type)
        };
    }

    public static SqlTypeDefinition AsDateTime() =>
        ByType(SqlDbType.DateTime);

    public static SqlTypeDefinition AsSmallDateTime() =>
        ByType(SqlDbType.SmallDateTime);

    public static SqlTypeDefinition AsDate() =>
        ByType(SqlDbType.Date);

    public static SqlTypeDefinition AsTime(byte? precision = null)
    {
        SqlDbType type = SqlDbType.Time;
        if (precision is not null)
        {
            if (precision < 0 || precision > 7)
                throw new SqlTypeArgumentOutOfRangeException(type, "precision", precision.Value, 0, 7);
        }
        return new()
        {
            Type = type,
            Scale = precision, //From what I can tell, in sql server it is called precision, but in ADO.Net you set scale.
            TypeDeclaration = precision is not null ? $"{ToSql(type)}({precision})" : ToSql(type)
        };
    }
    #endregion

    /// <summary>
    /// Constructor for using MAX has been specified in place of sizing arguments.
    /// For use with VARCHAR, NVARCHAR and VARBINARY.
    /// Though if you set max to false, you can safely use it with other types.
    /// </summary>
    /// <param name="type">The Sql Server ADO.Net Type</param>
    /// <param name="useMax">
    /// Constructor for using MAX has been specified in place of sizing arguments.
    /// For use with VARCHAR, NVARCHAR and VARBINARY.
    /// Though if you set max to false, you can safely use it with other types.
    /// </param>
    public SqlTypeDefinition(SqlDbType type, bool useMax)
    {
        SqlTypeNotSupportedException.ValidateTypeIsSupported(type);
        Type = type;
        if (useMax)
            switch (type)
            {
                case SqlDbType.NVarChar: //Length
                case SqlDbType.VarBinary: //Length
                case SqlDbType.VarChar: //Length
                    TypeDeclaration = $"{ToSql(type)}(MAX)";
                    UseMax = useMax;
                    break;
                default:
                    throw new SqlTypeDoesNotSupportSizeException(type);
            }
        else //if useMax is false, fall back to default values.
            TypeDeclaration = ToSql(type);
    }

    /// <summary>
    /// Returns the default SQL Server type keyword for the provided <see cref="SqlDbType"/>,
    /// without any precision/scale/length suffix.
    /// </summary>
    public static string ToSql(SqlDbType type) => type switch
    {
        //Guid
        SqlDbType.UniqueIdentifier => "UNIQUEIDENTIFIER",

        //strings
        SqlDbType.NText => "NTEXT",
        SqlDbType.Text => "TEXT",
        SqlDbType.Char => "CHAR",
        SqlDbType.VarChar => "VARCHAR",
        SqlDbType.NVarChar => "NVARCHAR",
        SqlDbType.NChar => "NCHAR",

        //TODO: XML:Not supported yet. Add in the future after issues mapping resolved, or remove.
        //SqlDbType.Json => "NVARCHAR(MAX)", //NOTE: JSON is not currently a column type, it is most closely NVARCHAR(MAX)
        //SqlDbType.Xml => "NVARCHAR(MAX)", //currently uses .Net NVARCHAR(MAX)

        //byte[]
        SqlDbType.Binary => "BINARY",
        SqlDbType.Image => "IMAGE",
        SqlDbType.VarBinary => "VARBINARY",

        //bool
        SqlDbType.Bit => "BIT",

        //Integers
        SqlDbType.TinyInt => "TINYINT",
        SqlDbType.SmallInt => "SMALLINT",
        SqlDbType.Int => "INT",
        SqlDbType.BigInt => "BIGINT",

        //returns .Net float / double
        SqlDbType.Real => "REAL",
        SqlDbType.Float => "FLOAT",

        //returns .Net Decimals
        SqlDbType.Decimal => "DECIMAL",
        SqlDbType.Money => "MONEY",
        SqlDbType.SmallMoney => "SMALLMONEY",

        //returns .Net DateTime
        SqlDbType.DateTime => "DATETIME",
        SqlDbType.SmallDateTime => "SMALLDATETIME",
        SqlDbType.DateTime2 => "DATETIME2",
        SqlDbType.Date => "DATE",

        SqlDbType.Time => "TIME", //.Net TimeSpan
        SqlDbType.DateTimeOffset => "DATETIMEOFFSET",//.Net DateTimeOffset

        //.Net byte[]
        SqlDbType.Timestamp => "ROWVERSION",

        SqlDbType.Structured => "TABLE TYPE",

        SqlDbType.Variant => "SQL_VARIANT", //.Net return type varies.

        SqlDbType.Udt => "UDT", 
        _ => throw new SqlTypeNotSupportedException([type])
    };
}
