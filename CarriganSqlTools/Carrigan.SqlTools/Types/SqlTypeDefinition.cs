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

    /// <summary>
    /// For VarChar, NVarChar and VarBinary: sets the item to MAX
    /// </summary>
    public bool UseMax { get; init; } = false;

    /// <summary>
    /// This represents the text to declare the indicated type in SQL with the supplied sizing arguments.
    /// </summary>
    public string TypeDeclaration { get; init; } = string.Empty;

    /// <summary>
    /// Defualt constructor
    /// </summary>
    private SqlTypeDefinition()
    {
    }

    /// <summary>
    /// Constructor gets SQL DB Type from an object's value's type.
    /// </summary>
    /// <param name="value">the value to use to determine the type</param>
    public SqlTypeDefinition(object? value)
    {
        Type = SqlTypeCache.GetSqlDbTypeFromValue(value);
        TypeDeclaration = ToSql(Type);
    }

    /// <summary>
    /// Constructor gets default SQL Type for a CLR type.
    /// </summary>
    /// <param name="value">the clr type to use to determine the sql type</param>
    public SqlTypeDefinition(Type type)
    {
        Type = SqlTypeCache.GetSqlDbType(type);
        TypeDeclaration = ToSql(Type);
    }

    #region helper methods
    /// <summary>
    /// Test's a range and throw an exception if needed. 
    /// </summary>
    /// <param name="type">The SqlDbType is used to create the exception message only.</param>
    /// <param name="name">The parameter name is used to create the exception message only.</param>
    /// <param name="value">the value to test</param>
    /// <param name="min">the min range</param>
    /// <param name="max">the max range</param>
    /// <exception cref="SqlTypeArgumentOutOfRangeException"></exception>
    private static void EnsureRange(SqlDbType type, string name, int value, int min, int max)
    {
        if (value < min || value > max)
            throw new SqlTypeArgumentOutOfRangeException(type, name, value, min, max);
    }
    /// <summary>
    /// Test's a range and throw an exception if needed. 
    /// </summary>
    /// <remarks>
    /// Min is assumed to be 0
    /// </remarks>
    /// <param name="type">The SqlDbType is used to create the exception message only.</param>
    /// <param name="name">The parameter name is used to create the exception message only.</param>
    /// <param name="value">the value to test</param>
    /// <param name="max">the max range</param>
    /// <exception cref="SqlTypeArgumentOutOfRangeException"></exception>
    private static void EnsureRange(SqlDbType type, string name, byte value, byte max)
    {
        if (value > max)
            throw new SqlTypeArgumentOutOfRangeException(type, name, value, 0, max);
    }

    /// <summary>
    /// Helper method to uniformly set SQL types that have a size.
    /// </summary>
    /// <param name="type">The SqlDbType</param>
    /// <param name="size">the size</param>
    /// <param name="min">the min range</param>
    /// <param name="max">the max range</param>
    /// <returns>SqlTypeDefinition</returns>
    private static SqlTypeDefinition WithSize(SqlDbType type, int? size, int min, int max)
    {
        if (size is not null) EnsureRange(type, nameof(size), size.Value, min, max);
        return new() { Type = type, Size = size, TypeDeclaration = size is null ? ToSql(type) : $"{ToSql(type)}({size})" };
    }

    /// <summary>
    /// Helper method to uniformly set SQL types with no arguments.
    /// </summary>
    /// <param name="type">The SqlDbType</param>
    /// <returns>SqlTypeDefinition</returns>
    private static SqlTypeDefinition ByType(SqlDbType type) => new ()
    {
        Type = type,
        TypeDeclaration = ToSql(type)
    };

    /// <summary>
    /// Helper method to uniformly set SQL types as Max.
    /// </summary>
    /// <param name="type">The SqlDbType</param>
    /// <returns>SqlTypeDefinition</returns>
    private static SqlTypeDefinition AsMax(SqlDbType type) => new()
    {
        Type = type,
        UseMax = true,
        TypeDeclaration = $"{ToSql(type)}(MAX)"
    };
    #endregion

    #region UniqueIdentifier
    /// <summary>
    /// Creates and returns SqlTypeDefinition to represent <see cref="SqlDbType.UniqueIdentifier">
    /// </summary>
    /// <returns></returns>
    public static SqlTypeDefinition AsUniqueIdentifier() =>
        ByType(SqlDbType.UniqueIdentifier);
    #endregion

    #region Chars

    /// <summary>
    /// Creates and returns SqlTypeDefinition to represent <see cref="SqlDbType.Char">
    /// </summary>
    /// <param name="size">size</param>
    /// <returns></returns>
    public static SqlTypeDefinition AsChar(int? size = null) =>
        WithSize(SqlDbType.Char, size, 1, 8000);

    /// <summary>
    /// Creates and returns SqlTypeDefinition to represent <see cref="SqlDbType.NChar">
    /// </summary>
    /// <param name="size">size</param>
    /// <returns></returns>
    public static SqlTypeDefinition AsNChar(int? size = null) =>
        WithSize(SqlDbType.NChar, size, 1, 4000);

    /// <summary>
    /// Creates and returns SqlTypeDefinition to represent <see cref="SqlDbType.VarChar">
    /// </summary>
    /// <param name="size">size</param>
    /// <returns></returns>
    public static SqlTypeDefinition AsVarChar(int? size = null) =>
        WithSize(SqlDbType.VarChar, size, 1, 8000);

    /// <summary>
    /// Creates and returns SqlTypeDefinition to represent <see cref="SqlDbType.NVarChar">
    /// </summary>
    /// <param name="size">size</param>
    /// <returns></returns>
    public static SqlTypeDefinition AsNVarChar(int? size = null) =>
        WithSize(SqlDbType.NVarChar, size, 1, 4000);

    /// <summary>
    /// Creates and returns SqlTypeDefinition to represent <see cref="SqlDbType.AsVarChar"> with <c>MAX</c>
    /// </summary>
    /// <returns></returns>
    public static SqlTypeDefinition AsVarCharMax() =>
        AsMax(SqlDbType.VarChar);

    /// <summary>
    /// Creates and returns SqlTypeDefinition to represent <see cref="SqlDbType.NVarChar"> with  <c>MAX</c>
    /// </summary>
    /// <returns></returns>
    public static SqlTypeDefinition AsNVarCharMax() =>
        AsMax(SqlDbType.NVarChar);


    /// <summary>
    /// Creates and returns SqlTypeDefinition to represent <see cref="SqlDbType.Text">
    /// </summary>
    /// <remarks>
    /// Text is obsolete, but preserved for legacy databases. ObsoleteAttribute is intentionally not used here.
    /// </remarks>
    /// <returns></returns>
    public static SqlTypeDefinition AsText() =>
        ByType(SqlDbType.Text);

    /// <summary>
    /// Creates and returns SqlTypeDefinition to represent <see cref="SqlDbType.NText">
    /// </summary>
    /// <remarks>
    /// NText is obsolete, but preserved for legacy databases. ObsoleteAttribute is intentionally not used here.
    /// </remarks>
    /// <returns></returns>
    public static SqlTypeDefinition AsNText() =>
        ByType(SqlDbType.NText);

    #endregion

    #region Binary

    /// <summary>
    /// Creates and returns SqlTypeDefinition to represent <see cref="SqlDbType.Binary">
    /// </summary>
    /// <param name="size">size</param>
    /// <returns></returns>
    public static SqlTypeDefinition AsBinary(int? size = null) =>
        WithSize(SqlDbType.Binary, size, 1, 8000);

    /// <summary>
    /// Creates and returns SqlTypeDefinition to represent <see cref="SqlDbType.VarBinary">
    /// </summary>
    /// <param name="size">size</param>
    /// <returns></returns>
    public static SqlTypeDefinition AsVarBinary(int? size = null) =>
        WithSize(SqlDbType.VarBinary, size, 1, 8000);

    /// <summary>
    /// Creates and returns SqlTypeDefinition to represent <see cref="SqlDbType.VarBinary"> with  <c>MAX</c>
    /// </summary>
    /// <returns></returns>
    public static SqlTypeDefinition AsVarBinaryMax() =>
        AsMax(SqlDbType.VarBinary);

    /// <summary>
    /// Creates and returns SqlTypeDefinition to represent <see cref="SqlDbType.NText">
    /// </summary>
    /// <remarks>
    /// Image is obsolete, but preserved for legacy databases. ObsoleteAttribute is intentionally not used here.
    /// </remarks>
    /// <returns></returns>
    public static SqlTypeDefinition AsImage() =>
        ByType(SqlDbType.Image);
    #endregion

    #region Bit

    /// <summary>
    /// Creates and returns SqlTypeDefinition to represent <see cref="SqlDbType.Bit">
    /// </summary>
    /// <returns></returns>
    public static SqlTypeDefinition AsBit() =>
        ByType(SqlDbType.Bit);
    #endregion

    #region Integers
    /// <summary>
    /// Creates and returns SqlTypeDefinition to represent <see cref="SqlDbType.TinyInt">
    /// </summary>
    /// <returns></returns>
    public static SqlTypeDefinition AsTinyInt() =>
        ByType(SqlDbType.TinyInt);

    /// <summary>
    /// Creates and returns SqlTypeDefinition to represent <see cref="SqlDbType.SmallInt">
    /// </summary>
    /// <returns></returns>
    public static SqlTypeDefinition AsSmallInt() =>
        ByType(SqlDbType.SmallInt);

    /// <summary>
    /// Creates and returns SqlTypeDefinition to represent <see cref="SqlDbType.Int">
    /// </summary>
    /// <returns></returns>
    public static SqlTypeDefinition AsInt() =>
        ByType(SqlDbType.Int);

    /// <summary>
    /// Creates and returns SqlTypeDefinition to represent <see cref="SqlDbType.BigInt">
    /// </summary>
    /// <returns></returns>
    public static SqlTypeDefinition AsBigInt() =>
        ByType(SqlDbType.BigInt);
    #endregion

    #region Floating Point
    /// <summary>
    /// Creates and returns SqlTypeDefinition to represent <see cref="SqlDbType.Real">
    /// </summary>
    /// <returns></returns>
    public static SqlTypeDefinition AsReal() =>
        ByType(SqlDbType.Real);

    /// <summary>
    /// Creates and returns SqlTypeDefinition to represent <see cref="SqlDbType.Float">
    /// </summary>
    /// <param name="precision">size</param>
    /// <returns></returns>
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
    /// <summary>
    /// Creates and returns SqlTypeDefinition to represent <see cref="SqlDbType.Decimal">
    /// </summary>
    /// <returns></returns>
    public static SqlTypeDefinition AsDecimal() =>
        ByType(SqlDbType.Decimal);

    /// <summary>
    /// Creates and returns SqlTypeDefinition to represent <see cref="SqlDbType.Decimal">
    /// </summary>
    /// <param name="precision">size</param>
    /// <returns></returns>
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

    /// <summary>
    /// Creates and returns SqlTypeDefinition to represent <see cref="SqlDbType.Decimal">
    /// </summary>
    /// <param name="precision">size</param>
    /// <param name="scale">scale</param>
    /// <returns></returns>
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

    /// <summary>
    /// Creates and returns SqlTypeDefinition to represent <see cref="SqlDbType.Money">
    /// </summary>
    /// <returns></returns>
    public static SqlTypeDefinition AsMoney() =>
        ByType(SqlDbType.Money);
    /// <summary>
    /// Creates and returns SqlTypeDefinition to represent <see cref="SqlDbType.SmallMoney">
    /// </summary>
    /// <returns></returns>
    public static SqlTypeDefinition AsSmallMoney() =>
        ByType(SqlDbType.SmallMoney);
    #endregion

    #region DateTime

    /// <summary>
    /// Creates and returns SqlTypeDefinition to represent <see cref="SqlDbType.DateTime2">
    /// </summary>
    /// <param name="scale">fractionalSecondPrecision</param>
    /// <returns></returns>
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

    /// <summary>
    /// Creates and returns SqlTypeDefinition to represent <see cref="SqlDbType.DateTimeOffset">
    /// </summary>
    /// <param name="scale">fractionalSecondPrecision</param>
    /// <returns></returns>
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

    /// <summary>
    /// Creates and returns SqlTypeDefinition to represent <see cref="SqlDbType.DateTime">
    /// </summary>
    /// <returns></returns>
    public static SqlTypeDefinition AsDateTime() =>
        ByType(SqlDbType.DateTime);

    /// <summary>
    /// Creates and returns SqlTypeDefinition to represent <see cref="SqlDbType.SmallDateTime">
    /// </summary>
    /// <returns></returns>
    public static SqlTypeDefinition AsSmallDateTime() =>
        ByType(SqlDbType.SmallDateTime);

    /// <summary>
    /// Creates and returns SqlTypeDefinition to represent <see cref="SqlDbType.Date">
    /// </summary>
    /// <returns></returns>
    public static SqlTypeDefinition AsDate() =>
        ByType(SqlDbType.Date);

    /// <summary>
    /// Creates and returns SqlTypeDefinition to represent <see cref="SqlDbType.Time">
    /// </summary>
    /// <param name="scale">fractionalSecondPrecision</param>
    /// <returns></returns>
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
