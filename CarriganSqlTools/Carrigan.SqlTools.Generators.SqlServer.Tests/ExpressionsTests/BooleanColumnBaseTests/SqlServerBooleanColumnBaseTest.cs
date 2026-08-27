using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.PredicatesLogic;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests.BooleanColumnBaseTests;

public abstract class SqlServerBooleanColumnBaseTest<modelT> : BooleanColumnBaseTests<modelT> where modelT : class
{
    protected override ISqlDialects Dialect =>
        new SqlServerDialect();

    protected override string ExpectSqlFragment(string expectedColumnName) =>
        SchemaName is null
            ? string.Format("[{0}].[{1}]", TableName, expectedColumnName)
            : string.Format("[{0}].[{1}].[{2}]", SchemaName, TableName, expectedColumnName);

    protected override BooleanColumn<modelT> NewBooleanColumn(string propertyName) =>
        new(propertyName);
    protected override BooleanColumn<modelT> NewBooleanColumn(PropertyName propertyName) =>
        new(propertyName);



    protected override Predicates NewColumnAsBooleanExpression(string propertyName)
    {
        Column<modelT> column = new(propertyName);
        Predicates booleanExpression = column;
        return booleanExpression;
    }
    protected override Predicates NewColumnAsBooleanExpression(PropertyName propertyName)
    {
        Column<modelT> column = new(propertyName);
        Predicates booleanExpression = column;
        return booleanExpression;
    }

    protected override BooleanColumnBase<modelT> NewColumnAsBooleanColumnBase(string propertyName)
    {
        Column<modelT> column = new(propertyName);
        BooleanColumnBase<modelT> booleanExpression = column;
        return booleanExpression;
    }
    protected override BooleanColumnBase<modelT> NewColumnAsBooleanColumnBase(PropertyName propertyName)
    {
        Column<modelT> column = new(propertyName);
        BooleanColumnBase<modelT> booleanExpression = column;
        return booleanExpression;
    }

    protected override BooleanColumnBase<modelT> NewColumnAsBooleanColumn(string propertyName)
    {
        Column<modelT> column = new(propertyName);
        BooleanColumn<modelT> booleanExpression = column;
        return booleanExpression;
    }
    protected override BooleanColumnBase<modelT> NewColumnAsBooleanColumn(PropertyName propertyName)
    {
        Column<modelT> column = new(propertyName);
        BooleanColumn<modelT> booleanExpression = column;
        return booleanExpression;
    }
}
