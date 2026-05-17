using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Paging;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Types;
using System.Xml;
using System.Xml.Linq;

//IGNORE SPELLING: Npgsql

namespace Carrigan.SqlTools.Dialects.PostgreSql;

/// <summary>
/// Provides SQL dialect-specific formatting and rendering logic for PostgreSQL.
/// </summary>
public class PostgreSqlDialect : ISqlDialects
{
    /// <summary>
    /// Encloses the specified identifier in PostgreSQL double quotes.
    /// Embedded double quotes are escaped by doubling them.
    /// </summary>
    public string QuoteIdentifier(string identifier)
    {
        ArgumentNullException.ThrowIfNull(identifier);

        return $"\"{identifier.Replace("\"", "\"\"")}\"";
    }

    /// <summary>
    /// Generates a string representation of the specified PostgreSQL table,
    /// optionally qualified by schema.
    /// </summary>
    public string RenderTable(SchemaName? schemaName, TableName tableName) =>
        schemaName.IsNotNullOrEmpty()
            ? $"{QuoteIdentifier(schemaName)}.{QuoteIdentifier(tableName)}"
            : QuoteIdentifier(tableName);

    /// <summary>
    /// Renders the fully qualified PostgreSQL column name.
    /// </summary>
    public string RenderColumn(TableTag tableTag, ColumnName columnName, bool includeTable = true) =>
        includeTable && tableTag.ToString().IsNotNullOrEmpty()
            ? $"{tableTag}.{QuoteIdentifier(columnName)}"
            : QuoteIdentifier(columnName);

    /// <summary>
    /// Generates SQL fragments for an INSERT statement that returns inserted values
    /// by appending PostgreSQL's RETURNING clause.
    /// </summary>
    public IEnumerable<SqlFragment> GetInsertReturningFragments<T>(IEnumerable<SqlFragment> insertIntoFragments, IEnumerable<SqlFragment> insertValuesFragments, IEnumerable<ColumnInfo> columnInfo)
    {
        ArgumentNullException.ThrowIfNull(insertIntoFragments);
        ArgumentNullException.ThrowIfNull(insertValuesFragments);
        ArgumentNullException.ThrowIfNull(columnInfo);

        ColumnInfo[] columns = [.. columnInfo];

        IEnumerable<SqlFragment> fragments = insertIntoFragments
            .Append(SqlFragment.NewLine)
            .Concat(insertValuesFragments);

        if (columns.Length == 0)
        {
            return fragments.Concat([SqlFragment.Semicolon, SqlFragment.NewLine]);
        }
        else
        {
            return fragments
                .Append(SqlFragment.NewLine)
                .Append(new SqlFragmentText(ReturningColumns<T>(columns)))
                .Concat([SqlFragment.Semicolon, SqlFragment.NewLine]);
        }
    }

    private string ReturningColumns<T>(IEnumerable<ColumnInfo> columnInfo) =>
        $"RETURNING {string.Join(", ", columnInfo.Select(ReturningColumn<T>))}";

    private string ReturningColumn<T>(ColumnInfo columnInfo)
    {
        string resultColumnName = InvocationReflectorCache<T>.GetResultColumnName(columnInfo.PropertyInfo);
        string columnName = QuoteIdentifier(columnInfo.ColumnName);

        if (resultColumnName != columnInfo.ColumnName)
        {
            return $"{columnName} AS {QuoteIdentifier(resultColumnName)}";
        }

        return columnName;
    }

    /// <summary>
    /// Generates a PostgreSQL LIMIT/OFFSET paging clause.
    /// </summary>
    public SqlFragment RenderPaging(PagingBase paging) =>
        new SqlFragmentText
        (
            (paging.Offset, paging.Next) switch
            {
                (0u, 0u) => string.Empty,
                (0u, _) => $"LIMIT {paging.Next}",
                (_, 0u) => $"OFFSET {paging.Offset}",
                _ => $"LIMIT {paging.Next} OFFSET {paging.Offset}"
            }
        );

    /// <summary>
    /// Renders PostgreSQL's native positional parameter name.
    /// PostgreSQL/Npgsql positional parameters are 1-origin: $1, $2, $3, etc.
    /// </summary>
    public string RenderFinalParameterName(string baseParameterName, int parameterIndex) =>
        $"${parameterIndex}";

    /// <summary>
    /// Returns the default PostgreSQL field properties for a CLR type.
    /// </summary>
    public FieldProperties GetDefaultFieldPropertiesByClrType(Type type) =>
        PostgreSqlTypesProvider.FromClrType(type);

    /// <summary>
    /// Generates the PostgreSQL declaration for a field based on the provided field properties.
    /// </summary>
    public string RenderFieldProperties(FieldProperties fieldProperties)
    {
        ArgumentNullException.ThrowIfNull(fieldProperties);

        if (fieldProperties.ProviderTypeName.IsNullOrWhiteSpace())
        {
            return string.Empty;
        }

        string declaration = fieldProperties.ProviderTypeName.ToUpperInvariant();

        if (declaration == "VECTOR" && fieldProperties.Length is not null)
        {
            declaration += $"({fieldProperties.Length})";
        }
        else if (fieldProperties.Precision is not null && fieldProperties.Scale is not null)
        {
            declaration += $"({fieldProperties.Precision}, {fieldProperties.Scale})";
        }
        else if (fieldProperties.Precision is not null)
        {
            declaration += $"({fieldProperties.Precision})";
        }
        else if (fieldProperties.FractionalSecondsPrecision is not null)
        {
            declaration = RenderTemporalPrecision(declaration, fieldProperties.FractionalSecondsPrecision.Value);
        }
        else if (fieldProperties.Length is not null && RequiresLengthDeclaration(declaration))
        {
            declaration += $"({fieldProperties.Length})";
        }

        return $"{declaration} {(fieldProperties.IsNullable ? "NULL" : "NOT NULL")}";
    }

    private static bool RequiresLengthDeclaration(string providerTypeName) =>
        providerTypeName is
            "CHAR" or
            "CHARACTER" or
            "VARCHAR" or
            "CHARACTER VARYING" or
            "BIT" or
            "BIT VARYING" or
            "VARBIT";

    private static string RenderTemporalPrecision(string declaration, byte fractionalSecondsPrecision) =>
        declaration switch
        {
            "TIME WITH TIME ZONE" => $"TIME({fractionalSecondsPrecision}) WITH TIME ZONE",
            "TIME WITHOUT TIME ZONE" => $"TIME({fractionalSecondsPrecision}) WITHOUT TIME ZONE",
            "TIMESTAMP WITH TIME ZONE" => $"TIMESTAMP({fractionalSecondsPrecision}) WITH TIME ZONE",
            "TIMESTAMP WITHOUT TIME ZONE" => $"TIMESTAMP({fractionalSecondsPrecision}) WITHOUT TIME ZONE",
            _ => $"{declaration}({fractionalSecondsPrecision})"
        };

    public SqlQuery RenderSqlQuery(IEnumerable<SqlFragment> sqlFragments) =>
        new(this, sqlFragments);

    public SqlQuery RenderStoredProcedureQuery(IEnumerable<SqlFragmentParameter> sqlFragments, ProcedureTag procedureTag) =>
        new(this, sqlFragments, procedureTag);

    /// <summary>
    /// Performs PostgreSQL parameter value conversions.
    /// </summary>
    public object ValueConversion(object? value)
    {
        if (value == null)
        {
            return DBNull.Value;
        }
        else if (value is XDocument xDocument)
        {
            return xDocument.ToString();
        }
        else if (value is XmlDocument xmlDocument)
        {
            return ((object?)xmlDocument.OuterXml) ?? DBNull.Value;
        }
        else
        {
            return value;
        }
    }

    public SqlFragment GetXOrSymbol() =>
        new SqlFragmentText("#");

    public SqlFragment GetDialectLike(bool? isCaseSensitive = null)
    {
        if (isCaseSensitive is null || isCaseSensitive.Value)
            return new SqlFragmentText("LIKE");
        else
            return new SqlFragmentText("ILIKE");
    }
}