using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.PredicatesLogic;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests.ColumnBaseTests;

public abstract class SqlServerColumnBaseTest<modelT> : ColumnBaseTests<modelT> where modelT : class
{
    protected override ISqlDialects Dialect =>
        new SqlServerDialect();

    protected override string ExpectSqlFragment(string expectedColumnName) =>
        SchemaName is null
            ? string.Format("[{0}].[{1}]", TableName, expectedColumnName)
            : string.Format("[{0}].[{1}].[{2}]", SchemaName, TableName, expectedColumnName);

    protected NumericColumn<modelT> NewNumericColumn(string propertyName) =>
        new(propertyName);
    protected NumericColumn<modelT> NewNumericColumn(PropertyName propertyName) =>
        new(propertyName);

    protected BooleanColumn<modelT> NewBooleanColumn(string propertyName) =>
        new(propertyName);
    protected BooleanColumn<modelT> NewBooleanColumn(PropertyName propertyName) =>
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


    protected override ColumnBase NewBooleanColumnModelTypeToColumnModelType(string propertyName)
    {
        BooleanColumn<modelT> booleanColumn = NewBooleanColumn(propertyName);
        Column<modelT> column = booleanColumn;
        return column;
    }
    protected override ColumnBase NewBooleanColumnModelTypeToColumnModelType(PropertyName propertyName)
    {
        BooleanColumn<modelT> booleanColumn = NewBooleanColumn(propertyName);
        Column<modelT> column = booleanColumn;
        return column;
    }

    protected override ColumnBase NewBooleanColumnModelTypeToColumnBaseModelType(string propertyName)
    {
        BooleanColumn<modelT> booleanColumn = NewBooleanColumn(propertyName);
        ColumnBase<modelT> column = booleanColumn;
        return column;
    }
    protected override ColumnBase NewBooleanColumnModelTypeToColumnBaseModelType(PropertyName propertyName)
    {
        BooleanColumn<modelT> booleanColumn = NewBooleanColumn(propertyName);
        ColumnBase<modelT> column = booleanColumn;
        return column;
    }

    protected override ColumnBase NewBooleanColumnModelTypeToColumnBase(string propertyName)
    {
        BooleanColumn<modelT> booleanColumn = NewBooleanColumn(propertyName);
        ColumnBase column = booleanColumn;
        return column;
    }
    protected override ColumnBase NewBooleanColumnModelTypeToColumnBase(PropertyName propertyName)
    {
        BooleanColumn<modelT> booleanColumn = NewBooleanColumn(propertyName);
        ColumnBase column = booleanColumn;
        return column;
    }

    protected override ColumnBase NewBooleanColumnBaseModelTypeToColumnModelType(string propertyName)
    {
        BooleanColumn<modelT> booleanColumn = NewBooleanColumn(propertyName);
        BooleanColumnBase<modelT> booleanColumnBase = booleanColumn;
        Column<modelT> column = booleanColumnBase;
        return column;
    }
    protected override ColumnBase NewBooleanColumnBaseModelTypeToColumnModelType(PropertyName propertyName)
    {
        BooleanColumn<modelT> booleanColumn = NewBooleanColumn(propertyName);
        BooleanColumnBase<modelT> booleanColumnBase = booleanColumn;
        Column<modelT> column = booleanColumnBase;
        return column;
    }
}
