using Carrigan.SqlTools.Dialects.PostgreSql;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using System.Reflection;

namespace Carrigan.SqlTools.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlDialectTests
{
    private sealed class InsertCustomerModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    [Fact]
    public void GetInsertReturningFragments_ReturningOneColumn_AppendsReturningClause()
    {
        SqlFragment[] insertIntoFragments = [new SqlFragmentText("INSERT INTO \"Customer\" (\"Name\")")];
        SqlFragment[] insertValuesFragments = [new SqlFragmentText("VALUES ($1)")];
        ColumnInfo[] columnInfo = [CreateColumnInfo<InsertCustomerModel>(nameof(InsertCustomerModel.Id))];

        string actual = Dialect.GetInsertReturningFragments<InsertCustomerModel>(insertIntoFragments, insertValuesFragments, columnInfo).ToSql(Dialect);

        string expected =
            "INSERT INTO \"Customer\" (\"Name\")" + Environment.NewLine +
            "VALUES ($1)" + Environment.NewLine +
            "RETURNING \"Id\";" + Environment.NewLine;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetInsertReturningFragments_ReturningMultipleColumns_AppendsReturningClause()
    {
        SqlFragment[] insertIntoFragments = [new SqlFragmentText("INSERT INTO \"Customer\" (\"Name\")")];
        SqlFragment[] insertValuesFragments = [new SqlFragmentText("VALUES ($1)")];
        ColumnInfo[] columnInfo =
        [
            CreateColumnInfo<InsertCustomerModel>(nameof(InsertCustomerModel.Id)),
            CreateColumnInfo<InsertCustomerModel>(nameof(InsertCustomerModel.Name))
        ];

        string actual = Dialect.GetInsertReturningFragments<InsertCustomerModel>(insertIntoFragments, insertValuesFragments, columnInfo).ToSql(Dialect);

        string expected =
            "INSERT INTO \"Customer\" (\"Name\")" + Environment.NewLine +
            "VALUES ($1)" + Environment.NewLine +
            "RETURNING \"Id\", \"Name\";" + Environment.NewLine;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetInsertReturningFragments_NoReturningColumns_ReturnsInsertStatementOnly()
    {
        SqlFragment[] insertIntoFragments = [new SqlFragmentText("INSERT INTO \"Customer\" (\"Name\")")];
        SqlFragment[] insertValuesFragments = [new SqlFragmentText("VALUES ($1)")];
        ColumnInfo[] columnInfo = [];

        string actual = Dialect.GetInsertReturningFragments<InsertCustomerModel>(insertIntoFragments, insertValuesFragments, columnInfo).ToSql(Dialect);

        string expected =
            "INSERT INTO \"Customer\" (\"Name\")" + Environment.NewLine +
            "VALUES ($1);" + Environment.NewLine;

        Assert.Equal(expected, actual);
    }

    private static ColumnInfo CreateColumnInfo<T>(string propertyName)
    {
        PropertyInfo propertyInfo = typeof(T).GetProperty(propertyName)!;
        return new ColumnInfo(Dialect, null, new TableName("Customer"), propertyInfo, []);
    }
}
