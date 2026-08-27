using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.PredicatesLogic;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests.NumericColumnBaseTests;

public abstract class SqlServerNumericColumnBaseTest<modelT> : NumericColumnBaseTests<modelT> where modelT : class
{
    protected override ISqlDialects Dialect =>
        new SqlServerDialect();

    protected override string ExpectSqlFragment(string expectedColumnName) =>
        SchemaName is null
            ? string.Format("[{0}].[{1}]", TableName, expectedColumnName)
            : string.Format("[{0}].[{1}].[{2}]", SchemaName, TableName, expectedColumnName);

    protected override NumericColumn<modelT> NewNumericColumn(string propertyName) =>
        new(propertyName);
    protected override NumericColumn<modelT> NewNumericColumn(PropertyName propertyName) =>
        new(propertyName);



    protected override NumericExpression NewColumnAsNumericExpression(string propertyName)
    {
        Column<modelT> column = new(propertyName);
        NumericExpression numericExpression = column;
        return numericExpression;
    }
    protected override NumericExpression NewColumnAsNumericExpression(PropertyName propertyName)
    {
        Column<modelT> column = new(propertyName);
        NumericExpression numericExpression = column;
        return numericExpression;
    }

    protected override NumericColumnBase<modelT> NewColumnAsNumericColumnBase(string propertyName)
    {
        Column<modelT> column = new(propertyName);
        NumericColumnBase<modelT> numericExpression = column;
        return numericExpression;
    }
    protected override NumericColumnBase<modelT> NewColumnAsNumericColumnBase(PropertyName propertyName)
    {
        Column<modelT> column = new(propertyName);
        NumericColumnBase<modelT> numericExpression = column;
        return numericExpression;
    }

    protected override NumericColumnBase<modelT> NewColumnAsNumericColumn(string propertyName)
    {
        Column<modelT> column = new(propertyName);
        NumericColumn<modelT> numericExpression = column;
        return numericExpression;
    }
    protected override NumericColumnBase<modelT> NewColumnAsNumericColumn(PropertyName propertyName)
    {
        Column<modelT> column = new(propertyName);
        NumericColumn<modelT> numericExpression = column;
        return numericExpression;
    }
}
