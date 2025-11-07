using Carrigan.SqlTools.Exceptions;
using System.Data;
using System.Drawing;
using System.Text;

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

    //TODO: Documentation, 
    public byte? Scale { get; init; } = null;

    public bool UseMax { get; init; } = false;

    /// <summary>
    /// This represents the text to declare the indicated type in SQL with the supplied sizing arguments.
    /// </summary>
    public string TypeDeclaration { get; init; } = string.Empty;


    private SqlTypeDefinition()
    {
    }

    /// <summary>
    /// Constructor to use when the type has no sizing arguments, or the default size is acceptable.
    /// </summary>
    /// <param name="type">The Sql Server ADO.Net Type</param>
    //public SqlTypeDefinition SqlTypeDefinition(SqlDbType type)
    //{
    //    SqlTypeNotSupportedException.ValidateTypeIsSupported(type);
    //    Type = type;
    //    TypeDeclaration = ToSql(type);
    //}
    #region UniqueIdentifier
    public static SqlTypeDefinition AsUniqueIdentifier()
    {
        SqlDbType type = SqlDbType.UniqueIdentifier;
        return new()
        {
            Type = type,
            TypeDeclaration = ToSql(type)
        };
    }
    #endregion

    #region Chars

    public static SqlTypeDefinition AsChar(int? size = null)
    {
        SqlDbType type = SqlDbType.Char;
        if (size is not null && (size < 1 || size > 8000))
            throw new SqlTypeArgumentOutOfRangeException(type, "size", size.Value, 1, 8000);
        return new SqlTypeDefinition()
        {
            Type = type,
            Size = size,
            TypeDeclaration = size is not null ? $"{ToSql(type)}({size})" : ToSql(type)
        };
    }

    public static SqlTypeDefinition AsNChar(int? size = null)
    {
        SqlDbType type = SqlDbType.NChar;
        if (size is not null && (size < 1 || size > 4000))
            throw new SqlTypeArgumentOutOfRangeException(type, "size", size.Value, 1, 4000);
        return new SqlTypeDefinition()
        {
            Type = type,
            Size = size,
            TypeDeclaration = size is not null ? $"{ToSql(type)}({size})" : ToSql(type)
        };
    }

    public static SqlTypeDefinition AsVarChar(int? size = null)
    {
        SqlDbType type = SqlDbType.VarChar;
        if (size is not null && (size < 1 || size > 8000))
            throw new SqlTypeArgumentOutOfRangeException(type, "size", size.Value, 1, 8000);
        return new SqlTypeDefinition()
        {
            Type = type,
            Size = size,
            TypeDeclaration = size is not null ? $"{ToSql(type)}({size})" : ToSql(type)
        };
    }

    public static SqlTypeDefinition AsNVarChar(int? size = null)
    {
        SqlDbType type = SqlDbType.NVarChar;
        if (size is not null && (size < 1 || size > 4000))
            throw new SqlTypeArgumentOutOfRangeException(type, "size", size.Value, 1, 4000);
        return new SqlTypeDefinition()
        {
            Type = type,
            Size = size,
            TypeDeclaration = size is not null ? $"{ToSql(type)}({size})" : ToSql(type)
        };
    }

    public static SqlTypeDefinition AsVarCharMax()
    {
        SqlDbType type = SqlDbType.VarChar;
        return new()
        {
            Type = type,
            UseMax = true,
            TypeDeclaration = $"{ToSql(type)}(MAX)"
        };
    }

    public static SqlTypeDefinition AsNVarCharMax()
    {
        SqlDbType type = SqlDbType.NVarChar;
        return new()
        {
            Type = type,
            UseMax = true,
            TypeDeclaration = $"{ToSql(type)}(MAX)"
        };
    }

    public static SqlTypeDefinition AsText()
    {
        SqlDbType type = SqlDbType.Text;
        return new()
        {
            Type = type,
            TypeDeclaration = ToSql(type)
        };
    }

    public static SqlTypeDefinition AsNText()
    {
        SqlDbType type = SqlDbType.NText;
        return new()
        {
            Type = type,
            TypeDeclaration = ToSql(type)
        };
    }

    #endregion

    #region Binary

    public static SqlTypeDefinition AsBinary(int? size = null)
    {
        SqlDbType type = SqlDbType.Binary;
        if (size is not null && (size < 1 || size > 8000))
            throw new SqlTypeArgumentOutOfRangeException(type, "size", size.Value, 1, 8000);
        return new SqlTypeDefinition()
        {
            Type = type,
            Size = size,
            TypeDeclaration = size is not null ? $"{ToSql(type)}({size})" : ToSql(type)
        };
    }

    public static SqlTypeDefinition AsVarBinary(int? size = null)
    {
        SqlDbType type = SqlDbType.Binary;
        if (size is not null && (size < 1 || size > 8000))
            throw new SqlTypeArgumentOutOfRangeException(type, "size", size.Value, 1, 8000);
        return new SqlTypeDefinition()
        {
            Type = type,
            Size = size,
            TypeDeclaration = size is not null ? $"{ToSql(type)}({size})" : ToSql(type)
        };
    }
    public static SqlTypeDefinition AsVarBinaryMax()
    {
        SqlDbType type = SqlDbType.VarBinary;
        return new()
        {
            Type = type,
            UseMax = true,
            TypeDeclaration = $"{ToSql(type)}(MAX)"
        };
    }

    public static SqlTypeDefinition AsImage()
    {
        SqlDbType type = SqlDbType.Image;
        return new()
        {
            Type = type,
            TypeDeclaration = ToSql(type)
        };
    }
    #endregion

    #region Bit

    public static SqlTypeDefinition AsBit()
    {
        SqlDbType type = SqlDbType.Bit;
        return new()
        {
            Type = type,
            TypeDeclaration = ToSql(type)
        };
    }
    #endregion

    #region Integers
    public static SqlTypeDefinition AsTinyInt()
    {
        SqlDbType type = SqlDbType.TinyInt;
        return new()
        {
            Type = type,
            TypeDeclaration = ToSql(type)
        };
    }

    public static SqlTypeDefinition AsSmallInt()
    {
        SqlDbType type = SqlDbType.SmallInt;
        return new()
        {
            Type = type,
            TypeDeclaration = ToSql(type)
        };
    }
    public static SqlTypeDefinition AsInt()
    {
        SqlDbType type = SqlDbType.Int;
        return new()
        {
            Type = type,
            TypeDeclaration = ToSql(type)
        };
    }

    public static SqlTypeDefinition AsBigInt()
    {
        SqlDbType type = SqlDbType.BigInt;
        return new()
        {
            Type = type,
            TypeDeclaration = ToSql(type)
        };
    }
    #endregion

    #region Floating Point
    public static SqlTypeDefinition AsReal()
    {
        SqlDbType type = SqlDbType.Real;
        return new()
        {
            Type = type,
            TypeDeclaration = ToSql(type)
        };
    }

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
    public static SqlTypeDefinition AsDecimal()
    {
        SqlDbType type = SqlDbType.Decimal;

        return new SqlTypeDefinition()
        {
            Type = type,
            TypeDeclaration = $"{ToSql(type)}"
        };
    }

    public static SqlTypeDefinition AsDecimal(byte precision)
    {
        SqlDbType type = SqlDbType.Decimal;

        if (precision < 1 || precision > 38)
            throw new SqlTypeArgumentOutOfRangeException(type, "precision", precision, 1, 38);

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

        if (precision < 1 || precision > 38)
            throw new SqlTypeArgumentOutOfRangeException(type, "precision", precision, 1, 38);

        if (scale > precision)
            throw new SqlTypeArgumentOutOfRangeException(type, "scale", scale, 0, precision);

        return new SqlTypeDefinition()
        {
            Type = type,
            Precision = precision,
            Scale = scale,
            TypeDeclaration = $"{ToSql(type)}({precision})({scale})"
        };
    }

    public static SqlTypeDefinition AsMoney()
    {
        SqlDbType type = SqlDbType.Money;
        return new()
        {
            Type = type,
            TypeDeclaration = ToSql(type)
        };
    }
    public static SqlTypeDefinition AsSmallMoney()
    {
        SqlDbType type = SqlDbType.SmallMoney;
        return new()
        {
            Type = type,
            TypeDeclaration = ToSql(type)
        };
    }
    #endregion

    #region DateTime

    public static SqlTypeDefinition AsDateTime2(byte? precision = null)
    {
        SqlDbType type = SqlDbType.DateTime2;
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

    public static SqlTypeDefinition AsDateTime()
    {
        SqlDbType type = SqlDbType.DateTime;
        return new()
        {
            Type = type,
            TypeDeclaration = ToSql(type)
        };
    }

    public static SqlTypeDefinition AsSmallDateTime()
    {
        SqlDbType type = SqlDbType.SmallDateTime;
        return new()
        {
            Type = type,
            TypeDeclaration = ToSql(type)
        };
    }

    public static SqlTypeDefinition AsDate()
    {
        SqlDbType type = SqlDbType.Date;
        return new()
        {
            Type = type,
            TypeDeclaration = ToSql(type)
        };
    }

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
    /// Constructor to use when specifying Size, Precision or Scale.
    /// </summary>
    /// <param name="type">The Sql Server ADO.Net Type</param>
    /// <param name="size">
    /// Parameter to use to specify a size argument.
    /// SQL Types with Size:
    /// Binary (1 to 8000)
    /// Char (1 to 8000)
    /// NChar (1 to 4000)
    /// NVarChar (1 to 4000)
    /// VarBinary (1 to 8000)
    /// VarChar (1 to 8000)
    /// </param>
    /// <param name="precision">
    /// Parameter to use to specify a precision argument.
    /// SQL Types with Precision:
    /// Float (1 to 53)
    /// Decimal (1 to 38)
    /// </param>
    /// <param name="scale">
    /// Parameter to use to specify a scale argument.
    /// SQL Types with Scale:
    /// Time (0 to 7)
    /// DateTime2 (0 to 7)
    /// DateTimeOffset (0 to 7)
    /// </param>
    public SqlTypeDefinition(SqlDbType type, int? size = null, byte? precision = null, byte? scale = null)
    {
        SqlTypeNotSupportedException.ValidateTypeIsSupported(type);
        Type = type;
        StringBuilder typeDeclarationBuilder = new(ToSql(type));

        if (size is not null)
        {
            switch (type)
            {
                case SqlDbType.Binary: //Length
                case SqlDbType.Char: //Length
                case SqlDbType.VarBinary: //Length
                case SqlDbType.VarChar: //Length
                    if (size < 1 || size > 8000)
                        throw new SqlTypeArgumentOutOfRangeException(type, "size", size.Value, 1, 8000);
                    Size = size;
                    break;
                case SqlDbType.NChar: //Length
                case SqlDbType.NVarChar: //Length
                    if (size < 1 || size > 4000)
                        throw new SqlTypeArgumentOutOfRangeException(type, "size", size.Value, 1, 4000);
                    Size = size;
                    break;
                default:
                    throw new SqlTypeDoesNotSupportSizeException(type);
            }
            typeDeclarationBuilder.Append($"({size})");
        }
        if(precision is not null)
        {
            if (type == SqlDbType.Float)
            {
                if (precision < 1 || precision > 53)
                    throw new SqlTypeArgumentOutOfRangeException(type, "precision", precision.Value, 1, 53);
                Precision = precision;
            }
            else if (type == SqlDbType.Decimal)
            {
                if (precision < 1 || precision > 38)
                    throw new SqlTypeArgumentOutOfRangeException(type, "precision", precision.Value, 1, 38);
                Precision = precision;
            }
            else
                throw new SqlTypeDoesNotSupportPrecisionException(type);

            typeDeclarationBuilder.Append($"({precision})");
        }
        if (scale is not null)
        {
            switch (type)
            {
                case SqlDbType.Decimal: //Precision & Scale
                    if (scale < 0 || scale > 38)
                        throw new SqlTypeArgumentOutOfRangeException(type, "scale", scale.Value, 0, 38);
                    Scale = scale;
                    break;
                case SqlDbType.Time: //Scale
                case SqlDbType.DateTime2: //Scale
                case SqlDbType.DateTimeOffset: //Scale
                    if (scale < 0 || scale > 7)
                        throw new SqlTypeArgumentOutOfRangeException(type, "scale", scale.Value, 0, 7);
                    Scale = scale;
                    break;
                default:
                    throw new SqlTypeDoesNotSupportScaleException(type);
            }
            typeDeclarationBuilder.Append($"({scale})");
        }
        TypeDeclaration = typeDeclarationBuilder.ToString();
    }

    /// <summary>
    /// Attribute constructor to use of Max has been specified in place of sizing arguments.
    /// For use with VARCHAR, NVARCHAR and VARBINARY.
    /// Though if you set max to false, you can safely use it with other types.
    /// </summary>
    /// <param name="type">The Sql Server ADO.Net Type</param>
    /// <param name="useMax">
    /// Attribute constructor to use of Max has been specified in place of sizing arguments.
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
