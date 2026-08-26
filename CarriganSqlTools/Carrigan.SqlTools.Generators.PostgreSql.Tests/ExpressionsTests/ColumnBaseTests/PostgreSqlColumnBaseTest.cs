using Carrigan.SqlTools.Base.Tests;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests.ColumnBaseTests;

public abstract class PostgreSqlColumnBaseTest<modelT> : ColumnBaseTests<modelT> where modelT : class
{
    protected override ISqlDialects Dialect =>
        new PostgreSqlDialect();

    protected override string ExpectSqlFragment(string expectedColumnName) =>
        SchemaName is null
            ? string.Format("\"{0}\".\"{1}\"", TableName, expectedColumnName)
            : string.Format("\"{0}\".\"{1}\".\"{2}\"", SchemaName, TableName, expectedColumnName);

    protected NumericColumn<modelT> NewNumericColumn(string propertyName) =>
        new(propertyName);
    protected NumericColumn<modelT> NewNumericColumn(PropertyName propertyName) =>
        new(propertyName);

    protected override ColumnBase NewNumericColumnModelTypeToColumnModelType(string propertyName)
    {
        NumericColumn<modelT> NumericColumn = NewNumericColumn(propertyName);
        Column<modelT> column = NumericColumn;
        return column;
    }
    protected override ColumnBase NewNumericColumnModelTypeToColumnModelType(PropertyName propertyName)
    {
        NumericColumn<modelT> NumericColumn = NewNumericColumn(propertyName);
        Column<modelT> column = NumericColumn;
        return column;
    }

    protected override ColumnBase NewNumericColumnModelTypeToColumnBaseModelType(string propertyName)
    {
        NumericColumn<modelT> NumericColumn = NewNumericColumn(propertyName);
        ColumnBase<modelT> column = NumericColumn;
        return column;
    }
    protected override ColumnBase NewNumericColumnModelTypeToColumnBaseModelType(PropertyName propertyName)
    {
        NumericColumn<modelT> NumericColumn = NewNumericColumn(propertyName);
        ColumnBase<modelT> column = NumericColumn;
        return column;
    }

    protected override ColumnBase NewNumericColumnModelTypeToColumnBase(string propertyName)
    {
        NumericColumn<modelT> NumericColumn = NewNumericColumn(propertyName);
        ColumnBase column = NumericColumn;
        return column;
    }
    protected override ColumnBase NewNumericColumnModelTypeToColumnBase(PropertyName propertyName)
    {
        NumericColumn<modelT> NumericColumn = NewNumericColumn(propertyName);
        ColumnBase column = NumericColumn;
        return column;
    }

    protected override ColumnBase NewNumericColumnBaseModelTypeToColumnModelType(string propertyName)
    {
        NumericColumn<modelT> NumericColumn = NewNumericColumn(propertyName);
        NumericColumnBase<modelT> numericColumnBase = NumericColumn;
        Column<modelT> column = numericColumnBase;
        return column;
    }
    protected override ColumnBase NewNumericColumnBaseModelTypeToColumnModelType(PropertyName propertyName)
    {
        NumericColumn<modelT> NumericColumn = NewNumericColumn(propertyName);
        NumericColumnBase<modelT> numericColumnBase = NumericColumn;
        Column<modelT> column = numericColumnBase;
        return column;
    }
}
