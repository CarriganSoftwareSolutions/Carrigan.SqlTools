using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.Types;
using System.Numerics;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents SQL's <c>CAST</c> expression, which converts an expression from one data type to another.
/// </summary>
//TODO: Examples
public class Cast : SqlExpression
{
    /// <summary>
    /// Initializes a <c>CAST(expression AS type)</c> expression.
    /// </summary>
    public SqlExpression SqlExpression { get; init; }
    /// <summary>
    /// Initializes the properties of the field to which the expression is being cast.
    /// </summary>
    public FieldProperties FieldProperties { get; init; }

    /// <summary>
    /// Initializes a <c>CAST(expression AS type)</c> expression.
    /// </summary>
    /// <param name="sqlExpression">
    /// The expression to cast.
    /// </param>
    /// <param name="fieldProperties">
    /// The properties of the field to which the expression is being cast.
    /// </param>
    public Cast(SqlExpression sqlExpression, FieldProperties fieldProperties) : base([sqlExpression])
    {
        SqlExpression = sqlExpression;
        FieldProperties = fieldProperties;
    }

    protected override bool EqualsCore(SqlExpression other) =>
        other is Cast cast && base.EqualsCore(other) &&  FieldPropertiesEqual(FieldProperties, cast.FieldProperties);

    protected override void AddToHashCode(ref HashCode hashCode)
    {
        base.AddToHashCode(ref hashCode);
        AddFieldPropertiesToHashCode(ref hashCode, FieldProperties);
    }

    private static bool FieldPropertiesEqual(FieldProperties left, FieldProperties right) =>
        left.Length == right.Length &&
        left.IsMax == right.IsMax &&
        left.IsUnicode == right.IsUnicode &&
        left.IsFixedLength == right.IsFixedLength &&
        left.Precision == right.Precision &&
        left.Scale == right.Scale &&
        left.FractionalSecondsPrecision == right.FractionalSecondsPrecision &&
        left.IsNullable == right.IsNullable &&
        string.Equals(left.ProviderTypeName, right.ProviderTypeName, StringComparison.OrdinalIgnoreCase) &&
        string.Equals(left.BaseType, right.BaseType, StringComparison.OrdinalIgnoreCase) &&
        left.IsArray == right.IsArray;

    private static void AddFieldPropertiesToHashCode(ref HashCode hashCode, FieldProperties fieldProperties)
    {
        hashCode.Add(fieldProperties.Length);
        hashCode.Add(fieldProperties.IsMax);
        hashCode.Add(fieldProperties.IsUnicode);
        hashCode.Add(fieldProperties.IsFixedLength);
        hashCode.Add(fieldProperties.Precision);
        hashCode.Add(fieldProperties.Scale);
        hashCode.Add(fieldProperties.FractionalSecondsPrecision);
        hashCode.Add(fieldProperties.IsNullable);
        hashCode.Add(fieldProperties.ProviderTypeName, StringComparer.OrdinalIgnoreCase);
        hashCode.Add(fieldProperties.BaseType, StringComparer.OrdinalIgnoreCase);
        hashCode.Add(fieldProperties.IsArray);
    }

    /// <summary>
    /// Determines whether the cast expression is valid in an aggregate SELECT list.
    /// </summary>
    /// The optional <c>GROUP BY</c> clause used to validate grouped column expressions.
    /// <returns>
    /// The aggregate status of the expression being cast.
    /// </returns>
    public override bool IsAggregate() =>
        SqlExpression.IsAggregate();

    /// <summary>
    /// Generates the SQL fragments for the <c>CAST</c> expression based on the specified SQL dialect.
    /// </summary>
    /// <param name="dialect">
    /// The SQL dialect to use for rendering the <c>CAST</c> expression.
    /// </param>
    /// <returns>
    /// An enumerable of <see cref="ISqlFragment"/> representing the <c>CAST</c> expression in the specified SQL dialect.
    /// </returns>
    public override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect)
    {
        yield return new SqlFragmentText("CAST(");
        foreach(ISqlFragment sqlFragment in SqlExpression.ToSqlFragments(dialect))
        {
            yield return sqlFragment;
        }
        yield return new SqlFragmentText($" AS {dialect.RenderCastType(FieldProperties)}");
        yield return new SqlFragmentText(")");
    }
}
