using Carrigan.SqlTools.Exceptions;
using System.Data;
using System.Drawing;
using System.Text;

//IGNORE SPELLING: xml, unicode
//TODO: proof read  documentation 
namespace Carrigan.SqlTools.SqlGenerators;

/// <summary>
/// Extensions that map <see cref="SqlDbType"/> to SQL Server type keywords,
/// optionally validating and applying precision/scale (or length/MAX where applicable).
/// </summary>
public class SqlTypeDefinition
{
    /// <summary>
    /// The Sql Server ADO.Net Type
    /// </summary>
    public SqlDbType Type { get; }

    //TODO: Documentation, 
    public int? Size { get; } = null;

    //TODO: Documentation,
    public byte? Precision { get; } = null;

    //TODO: Documentation, 
    public byte? Scale { get; } = null;

    public bool UseMax { get; } = false;

    /// <summary>
    /// This represents the text to declare the indicated type in SQL with the supplied sizing arguments.
    /// </summary>
    public readonly string TypeDeclaration;


    /// <summary>
    /// Constructor to use when the type has no sizing arguments, or the default size is acceptable.
    /// </summary>
    /// <param name="type">The Sql Server ADO.Net Type</param>
    public SqlTypeDefinition(SqlDbType type)
    {
        SqlTypeNotSupportedException.ValidateTypeIsSupported(type);
        Type = type;
        TypeDeclaration = ToSql(type);
    }

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
