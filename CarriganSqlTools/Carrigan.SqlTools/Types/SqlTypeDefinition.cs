using Carrigan.SqlTools.Exceptions;
using System.Data;
using System.Text;

//IGNORE SPELLING: xml, unicode
//TODO:  documentation 
namespace Carrigan.SqlTools.Types;

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

    public SqlTypeDefinition(object? value)
    {
        Type = SqlTypeCache.GetSqlDbTypeFromValue(value);
        TypeDeclaration = ToSql(Type);
    }

    public SqlTypeDefinition(Type type)
    {
        Type = SqlTypeCache.GetSqlDbType(type);
        TypeDeclaration = ToSql(Type);
    }

    #region helper methods
    private static void EnsureRange(SqlDbType type, string name, int value, int min, int max)
    {
        if (value < min || value > max)
            throw new SqlTypeArgumentOutOfRangeException(type, name, value, min, max);
    }
    private static void EnsureRange(SqlDbType type, string name, byte value, byte max)
    {
        if (value > max)
            throw new SqlTypeArgumentOutOfRangeException(type, name, value, 0, max);
    }

    private static SqlTypeDefinition WithSize(SqlDbType type, int? size, int min, int max)
    {
        if (size is not null) EnsureRange(type, nameof(size), size.Value, min, max);
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
        UseMax = true,
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

    //Note:Text is obsolete, but preserved for legacy databases. ObsoleteAttribute is intentionally not used here.
    public static SqlTypeDefinition AsText() =>
        ByType(SqlDbType.Text);

    //Note:NText is obsolete, but preserved for legacy databases. ObsoleteAttribute is intentionally not used here.
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

    //Note:Image is obsolete, but preserved for legacy databases. ObsoleteAttribute is intentionally not used here.
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
            throw new SqlTypeArgumentOutOfRangeException(type, nameof(precision), precision.Value, 1, 53);
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

        EnsureRange(type, nameof(precision), precision, 1, 38);

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

        EnsureRange(type, nameof(precision), precision, 1, 38);

        EnsureRange(type, nameof(scale), scale, precision);

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
            EnsureRange(type, nameof(fractionalSecondPrecision), fractionalSecondPrecision.Value, 7);
        }
        return new()
        {
            Type = type,
            Scale = fractionalSecondPrecision, //From what I can tell, in sql server it is called precision, but in ADO.Net you set scale.
            TypeDeclaration = fractionalSecondPrecision is not null ? $"{ToSql(type)}({fractionalSecondPrecision})" : ToSql(type)
        };
    }

    public static SqlTypeDefinition AsDateTimeOffset(byte? fractionalSecondPrecision = null)
    {
        SqlDbType type = SqlDbType.DateTimeOffset;
        if (fractionalSecondPrecision is not null)
        {
            EnsureRange(type, nameof(fractionalSecondPrecision), fractionalSecondPrecision.Value, 7);
        }
        return new()
        {
            Type = type,
            Scale = fractionalSecondPrecision, //From what I can tell, in sql server it is called precision, but in ADO.Net you set scale.
            TypeDeclaration = fractionalSecondPrecision is not null ? $"{ToSql(type)}({fractionalSecondPrecision})" : ToSql(type)
        };
    }

    public static SqlTypeDefinition AsDateTime() =>
        ByType(SqlDbType.DateTime);

    public static SqlTypeDefinition AsSmallDateTime() =>
        ByType(SqlDbType.SmallDateTime);

    public static SqlTypeDefinition AsDate() =>
        ByType(SqlDbType.Date);

    public static SqlTypeDefinition AsTime(byte? fractionalSecondPrecision = null)
    {
        SqlDbType type = SqlDbType.Time;
        if (fractionalSecondPrecision is not null)
        {
            EnsureRange(type, nameof(fractionalSecondPrecision), fractionalSecondPrecision.Value, 7);
        }
        return new()
        {
            Type = type,
            Scale = fractionalSecondPrecision, //From what I can tell, in sql server it is called precision, but in ADO.Net you set scale.
            TypeDeclaration = fractionalSecondPrecision is not null ? $"{ToSql(type)}({fractionalSecondPrecision})" : ToSql(type)
        };
    }
    #endregion

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

        SqlDbType.Variant => "SQL_VARIANT", //.Net return type varies.

        SqlDbType.Udt => throw new SqlTypeNotSupportedException([type]),

        SqlDbType.Structured => throw new SqlTypeNotSupportedException([type]),
        _ => throw new SqlTypeNotSupportedException([type])
    };
}
