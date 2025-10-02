using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.OrderByItems;
using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Tests.TestEntities;
using Carrigan.SqlTools.Tests.TestEntities.Attributes;

namespace Carrigan.SqlTools.Tests.AttributesTests;
public class ParameterAndColumnIdentifierTests
{
    private static readonly SqlGenerator<ColumnIdentifiers> _generator = new();
    private static readonly ColumnIdentifiers _entity = new ()
    {
        Id = 1,
        Property = 2,
        ColumnName = 3,
        IdentifierName = 4,
        IdentifierOverrideName = 5
    };
    private static readonly ColumnIdentifiers _model = new()
    {
        Id = 6,
        Property = 7,
        ColumnName = 8,
        IdentifierName = 9,
        IdentifierOverrideName = 10
    };

    private static readonly ColumnIdentifiers _updateValues = new()
    {
        Property = 12,
        ColumnName = 13,
        IdentifierName = 14,
        IdentifierOverrideName = 15
    };


    private static readonly IEnumerable<ColumnIdentifiers> _entities = [_entity, _model];

    [Fact]
    public void DeleteTest()
    {
        SqlQuery query = _generator.Delete(_entity);
        string actual = query.QueryText;
        string expected = "DELETE FROM [ColumnIdentifiers] WHERE [Id] = @IdParameter;";
        Assert.Equal(expected, actual);

        Assert.Equal(1, query.GetParameterCount());
        Assert.Equal(1, query.GetParameterValue<int>("IdParameter"));
    }

    [Fact]
    public void InsertTest()
    {
        SqlQuery query = _generator.Insert(_entity);
        string actual = query.QueryText;
        string expected = "INSERT INTO [ColumnIdentifiers] ([Id], [Property], [Column], [Identifier], [IdentifierOverride]) VALUES (@IdParameter, @PropertyParameter, @ColumnParameter, @IdentifierParameter, @IdentifierOverrideParameter);";
        Assert.Equal(expected, actual);

        Assert.Equal(5, query.GetParameterCount());
        Assert.Equal(1, query.GetParameterValue<int>("IdParameter"));
        Assert.Equal(2, query.GetParameterValue<int>("PropertyParameter"));
        Assert.Equal(3, query.GetParameterValue<int>("ColumnParameter"));
        Assert.Equal(4, query.GetParameterValue<int>("IdentifierParameter"));
        Assert.Equal(5, query.GetParameterValue<int>("IdentifierOverrideParameter"));
    }

    [Fact]
    public void InsertAutoIdTest()
    {
        SqlQuery query = _generator.InsertAutoId(_entity);
        string actual = query.QueryText;
        string expected = SqlGenerator<ColumnIdentifiers>.ModifyInsertQueryToReturnScalar("INSERT INTO [ColumnIdentifiers] ([Property], [Column], [Identifier], [IdentifierOverride]) VALUES (@PropertyParameter, @ColumnParameter, @IdentifierParameter, @IdentifierOverrideParameter);");
        Assert.Equal(expected, actual);

        Assert.Equal(4, query.GetParameterCount());
        Assert.Equal(2, query.GetParameterValue<int>("PropertyParameter"));
        Assert.Equal(3, query.GetParameterValue<int>("ColumnParameter"));
        Assert.Equal(4, query.GetParameterValue<int>("IdentifierParameter"));
        Assert.Equal(5, query.GetParameterValue<int>("IdentifierOverrideParameter"));
    }

    [Fact]
    public void UpdateByIdTest()
    {
        SqlQuery query = _generator.UpdateById(_entity);
        string actual = query.QueryText;
        string expected = "UPDATE [ColumnIdentifiers] SET [Property] = @PropertyParameter, [Column] = @ColumnParameter, [Identifier] = @IdentifierParameter, [IdentifierOverride] = @IdentifierOverrideParameter WHERE [Id] = @IdParameter;";
        Assert.Equal(expected, actual);

        Assert.Equal(5, query.GetParameterCount());
        Assert.Equal(1, query.GetParameterValue<int>("IdParameter"));
        Assert.Equal(2, query.GetParameterValue<int>("PropertyParameter"));
        Assert.Equal(3, query.GetParameterValue<int>("ColumnParameter"));
        Assert.Equal(4, query.GetParameterValue<int>("IdentifierParameter"));
        Assert.Equal(5, query.GetParameterValue<int>("IdentifierOverrideParameter"));
    }

    [Fact]
    public void UpdateByIdsTest()
    {
        SqlQuery query = _generator.UpdateByIds(_updateValues, null, _entities);
        string actual = query.QueryText;
        string expected = "UPDATE [ColumnIdentifiers] SET [ColumnIdentifiers].[Property] = @ParameterSet_PropertyParameter, [ColumnIdentifiers].[Column] = @ParameterSet_ColumnParameter, [ColumnIdentifiers].[Identifier] = @ParameterSet_IdentifierParameter, [ColumnIdentifiers].[IdentifierOverride] = @ParameterSet_IdentifierOverrideParameter FROM [ColumnIdentifiers] WHERE (([ColumnIdentifiers].[Id] = @Parameter_0_R_IdParameter) OR ([ColumnIdentifiers].[Id] = @Parameter_1_R_IdParameter))";
        Assert.Equal(expected, actual);

        Assert.Equal(6, query.GetParameterCount());
        Assert.Equal(1, query.GetParameterValue<int>("@Parameter_0_R_IdParameter"));
        Assert.Equal(6, query.GetParameterValue<int>("@Parameter_1_R_IdParameter"));
        Assert.Equal(12, query.GetParameterValue<int>("@ParameterSet_PropertyParameter"));
        Assert.Equal(13, query.GetParameterValue<int>("@ParameterSet_ColumnParameter"));
        Assert.Equal(14, query.GetParameterValue<int>("@ParameterSet_IdentifierParameter"));
        Assert.Equal(15, query.GetParameterValue<int>("@ParameterSet_IdentifierOverrideParameter"));
    }

    [Fact]
    public void JoinTest()
    {
        LeftJoin<ColumnIdentifiers, JoinRightTable> join = new(new ColumnEqualsColumn<ColumnIdentifiers, JoinRightTable>(nameof(ColumnIdentifiers.Id), nameof(JoinRightTable.Id)));
        SqlQuery query = _generator.Delete(join, null);
        string actual = query.QueryText;
        string expected = "DELETE FROM [ColumnIdentifiers] LEFT JOIN [Right] ON ([ColumnIdentifiers].[Id] = [Right].[Id])";
        Assert.Equal(expected, actual);

        Assert.Equal(0, query.GetParameterCount());
    }

    [Fact]
    public void LeftJoinTest()
    {
        LeftJoin<ColumnIdentifiers, JoinRightTable> join = new(new ColumnEqualsColumn<ColumnIdentifiers, JoinRightTable>(nameof(ColumnIdentifiers.Id), nameof(JoinRightTable.Id)));
        SqlQuery query = _generator.Delete(join, null);
        string actual = query.QueryText;
        string expected = "DELETE FROM [ColumnIdentifiers] LEFT JOIN [Right] ON ([ColumnIdentifiers].[Id] = [Right].[Id])";
        Assert.Equal(expected, actual);

        Assert.Equal(0, query.GetParameterCount());
    }

    [Fact]
    public void InnerJoinTest()
    {
        InnerJoin<ColumnIdentifiers, JoinRightTable> join = new(new ColumnEqualsColumn<ColumnIdentifiers, JoinRightTable>(nameof(ColumnIdentifiers.Id), nameof(JoinRightTable.Id)));
        SqlQuery query = _generator.Delete(join, null);
        string actual = query.QueryText;
        string expected = "DELETE FROM [ColumnIdentifiers] INNER JOIN [Right] ON ([ColumnIdentifiers].[Id] = [Right].[Id])";
        Assert.Equal(expected, actual);

        Assert.Equal(0, query.GetParameterCount());
    }

    [Fact]
    public void OrderByTest()
    {
        OrderByItem<ColumnIdentifiers> orderByItem = new(nameof(ColumnIdentifiers.ColumnName));
        SqlQuery query = _generator.Select(null, null, null, orderByItem, null);
        string actual = query.QueryText;
        string expected = "SELECT [ColumnIdentifiers].* FROM [ColumnIdentifiers] ORDER BY [ColumnIdentifiers].[Column] ASC";
        Assert.Equal(expected, actual);

        Assert.Equal(0, query.GetParameterCount());
    }

    [Fact]
    public void AndTest()
    {
        Columns<ColumnIdentifiers> identifierColumn = new(nameof(ColumnIdentifiers.IdentifierName));
        Parameters parameter1 = new(new ParameterTag(null, "p1", null), 1);
        Columns<ColumnIdentifiers> columnColumn = new(nameof(ColumnIdentifiers.ColumnName));
        Parameters parameter2 = new(new ParameterTag(null, "p2", null), 2);
        Equal equal1 = new(identifierColumn, parameter1);
        Equal equal2 = new(columnColumn, parameter2);
        And and = new (equal1, equal2);

        SqlQuery query = _generator.Select(null, null, and, null, null);
        string expectedSql =
            "SELECT [ColumnIdentifiers].* FROM [ColumnIdentifiers] WHERE (([ColumnIdentifiers].[Identifier] = @Parameter_p1) AND ([ColumnIdentifiers].[Column] = @Parameter_p2))";
        string actualSql = query.QueryText;
        Assert.Equal(expectedSql, actualSql);

        Assert.Equal(2, query.GetParameterCount());
        Assert.Equal(1, query.GetParameterValue<int>("@Parameter_p1"));
        Assert.Equal(2, query.GetParameterValue<int>("@Parameter_p2"));
    }

    [Fact]
    public void ColumnEqualsColumnTest()
    {
        ColumnEqualsColumn<ColumnIdentifiers, ColumnIdentifiers> columns = new (nameof(ColumnIdentifiers.IdentifierOverrideName), nameof(ColumnIdentifiers.ColumnName));

        SqlQuery query = _generator.Select(null, null, columns, null, null);
        string expectedSql =
            "SELECT [ColumnIdentifiers].* FROM [ColumnIdentifiers] WHERE ([ColumnIdentifiers].[IdentifierOverride] = [ColumnIdentifiers].[Column])";
        string actualSql = query.QueryText;
        Assert.Equal(expectedSql, actualSql);

        Assert.Equal(0, query.GetParameterCount());
    }

    [Fact]
    public void ColumnTest()
    {
        Columns<ColumnIdentifiers> identifierOverrideColumn = new(nameof(ColumnIdentifiers.IdentifierOverrideName));
        Parameters parameter =new(new ParameterTag(null, "p1", null), 1);
        Equal equal = new(identifierOverrideColumn, parameter);

        SqlQuery query = _generator.Select(null, null, equal, null, null);
        string expectedSql =
            "SELECT [ColumnIdentifiers].* FROM [ColumnIdentifiers] WHERE ([ColumnIdentifiers].[IdentifierOverride] = @Parameter_p1)";
        string actualSql = query.QueryText;
        Assert.Equal(expectedSql, actualSql);

        Assert.Equal(1, query.GetParameterCount());
        Assert.Equal(1, query.GetParameterValue<int>("@Parameter_p1"));
    }

    [Fact]
    public void ColumnValueTest()
    {
        ColumnValues<ColumnIdentifiers> whereIdEquals = new(nameof(ColumnIdentifiers.Id), 1);
        SqlQuery query = _generator.Delete(null, whereIdEquals);
        string actual = query.QueryText;
        string expected = "DELETE FROM [ColumnIdentifiers] WHERE ([ColumnIdentifiers].[Id] = @Parameter_IdParameter)";
        Assert.Equal(expected, actual);

        Assert.Equal(1, query.GetParameterCount());
        Assert.Equal(1, query.GetParameterValue<int>("@Parameter_IdParameter"));
    }

    [Fact]
    public void ContainsTest()
    {
        Columns<ColumnIdentifiers> column = new(nameof(ColumnIdentifiers.IdentifierName));
        Parameters parameter = new("p1", 1);
        Contains<ColumnIdentifiers> contains = new (column, parameter);

        SqlQuery query = _generator.Select(null, null, contains, null, null);
        string expected = "SELECT [ColumnIdentifiers].* FROM [ColumnIdentifiers] WHERE CONTAINS([ColumnIdentifiers].[Identifier], @Parameter_p1)";
        string actual = query.QueryText;

        Assert.Equal(expected, actual);
        Assert.Equal(1, query.GetParameterCount());
        Assert.Equal(1, query.GetParameterValue<int>("@Parameter_p1"));
    }

    [Fact]
    public void EqualTest()
    {
        Columns<ColumnIdentifiers> column = new(nameof(ColumnIdentifiers.IdentifierName));
        Parameters parameter = new("p1", 1);
        Equal equal = new(column, parameter);

        SqlQuery query = _generator.Select(null, null, equal, null, null);
        string expectedSql =
            "SELECT [ColumnIdentifiers].* FROM [ColumnIdentifiers] WHERE ([ColumnIdentifiers].[Identifier] = @Parameter_p1)";
        string actualSql = query.QueryText;
        Assert.Equal(expectedSql, actualSql);

        Assert.Equal(1, query.GetParameterCount());
        Assert.Equal(1, query.GetParameterValue<int>("@Parameter_p1"));
    }

    [Fact]
    public void GreaterThanEqualsTest()
    {
        Columns<ColumnIdentifiers> column = new(nameof(ColumnIdentifiers.IdentifierName));
        Parameters parameter = new("p1", 1);
        GreaterThanEquals op = new(column, parameter);

        SqlQuery query = _generator.Select(null, null, op, null, null);
        string expectedSql =
            "SELECT [ColumnIdentifiers].* FROM [ColumnIdentifiers] WHERE ([ColumnIdentifiers].[Identifier] >= @Parameter_p1)";
        string actualSql = query.QueryText;
        Assert.Equal(expectedSql, actualSql);

        Assert.Equal(1, query.GetParameterCount());
        Assert.Equal(1, query.GetParameterValue<int>("@Parameter_p1"));
    }

    [Fact]
    public void GreaterThanTest()
    {
        Columns<ColumnIdentifiers> column = new(nameof(ColumnIdentifiers.IdentifierName));
        Parameters parameter = new("p1", 1);
        GreaterThan op = new(column, parameter);

        SqlQuery query = _generator.Select(null, null, op, null, null);
        string expectedSql =
            "SELECT [ColumnIdentifiers].* FROM [ColumnIdentifiers] WHERE ([ColumnIdentifiers].[Identifier] > @Parameter_p1)";
        string actualSql = query.QueryText;
        Assert.Equal(expectedSql, actualSql);

        Assert.Equal(1, query.GetParameterCount());
        Assert.Equal(1, query.GetParameterValue<int>("@Parameter_p1"));
    }

    [Fact]
    public void IsNotNullTest()
    {
        Columns<ColumnIdentifiers> column = new(nameof(ColumnIdentifiers.IdentifierName));
        IsNotNull op = new(column);

        SqlQuery query = _generator.Select(null, null, op, null, null);
        string expectedSql =
            "SELECT [ColumnIdentifiers].* FROM [ColumnIdentifiers] WHERE ([ColumnIdentifiers].[Identifier] IS NOT NULL)";
        string actualSql = query.QueryText;
        Assert.Equal(expectedSql, actualSql);

        Assert.Equal(0, query.GetParameterCount());
    }

    [Fact]
    public void IsNullTest()
    {
        Columns<ColumnIdentifiers> column = new(nameof(ColumnIdentifiers.IdentifierName));
        IsNull op = new(column);

        SqlQuery query = _generator.Select(null, null, op, null, null);
        string expectedSql =
            "SELECT [ColumnIdentifiers].* FROM [ColumnIdentifiers] WHERE ([ColumnIdentifiers].[Identifier] IS NULL)";
        string actualSql = query.QueryText;
        Assert.Equal(expectedSql, actualSql);

        Assert.Equal(0, query.GetParameterCount());
    }

    

    [Fact]
    public void LessThanEqualsTest()
    {
        Columns<ColumnIdentifiers> column = new(nameof(ColumnIdentifiers.IdentifierName));
        Parameters parameter = new("p1", 1);
        LessThanEquals op = new(column, parameter);

        SqlQuery query = _generator.Select(null, null, op, null, null);
        string expectedSql =
            "SELECT [ColumnIdentifiers].* FROM [ColumnIdentifiers] WHERE ([ColumnIdentifiers].[Identifier] <= @Parameter_p1)";
        string actualSql = query.QueryText;
        Assert.Equal(expectedSql, actualSql);

        Assert.Equal(1, query.GetParameterCount());
        Assert.Equal(1, query.GetParameterValue<int>("@Parameter_p1"));
    }

    [Fact]
    public void LessThanTest()
    {
        Columns<ColumnIdentifiers> column = new(nameof(ColumnIdentifiers.IdentifierName));
        Parameters parameter = new("p1", 1);
        LessThan op = new(column, parameter);

        SqlQuery query = _generator.Select(null, null, op, null, null);
        string expectedSql =
            "SELECT [ColumnIdentifiers].* FROM [ColumnIdentifiers] WHERE ([ColumnIdentifiers].[Identifier] < @Parameter_p1)";
        string actualSql = query.QueryText;
        Assert.Equal(expectedSql, actualSql);

        Assert.Equal(1, query.GetParameterCount());
        Assert.Equal(1, query.GetParameterValue<int>("@Parameter_p1"));
    }

    [Fact]
    public void NotEqualTest()
    {
        Columns<ColumnIdentifiers> column = new(nameof(ColumnIdentifiers.IdentifierName));
        Parameters parameter = new("p1", 1);
        NotEqual op = new(column, parameter);

        SqlQuery query = _generator.Select(null, null, op, null, null);
        string expectedSql =
            "SELECT [ColumnIdentifiers].* FROM [ColumnIdentifiers] WHERE ([ColumnIdentifiers].[Identifier] <> @Parameter_p1)";
        string actualSql = query.QueryText;
        Assert.Equal(expectedSql, actualSql);

        Assert.Equal(1, query.GetParameterCount());
        Assert.Equal(1, query.GetParameterValue<int>("@Parameter_p1"));
    }

    [Fact]
    public void NotTest()
    {
        Columns<ColumnIdentifiers> column = new(nameof(ColumnIdentifiers.IdentifierName));
        Not op = new(column);

        SqlQuery query = _generator.Select(null, null, op, null, null);
        string expectedSql =
            "SELECT [ColumnIdentifiers].* FROM [ColumnIdentifiers] WHERE (NOT [ColumnIdentifiers].[Identifier])";
        string actualSql = query.QueryText;
        Assert.Equal(expectedSql, actualSql);

        Assert.Equal(0, query.GetParameterCount());
    }

    [Fact]
    public void OrTest()
    {
        Columns<ColumnIdentifiers> identifierColumn = new(nameof(ColumnIdentifiers.IdentifierName));
        Parameters parameter1 = new("p1", 1);
        Columns<ColumnIdentifiers> columnColumn = new(nameof(ColumnIdentifiers.ColumnName));
        Parameters parameter2 = new("p2", 2);
        Equal equal1 = new(identifierColumn, parameter1);
        Equal equal2 = new(columnColumn, parameter2);
        Or or = new(equal1, equal2);

        SqlQuery query = _generator.Select(null, null, or, null, null);
        string expectedSql =
            "SELECT [ColumnIdentifiers].* FROM [ColumnIdentifiers] WHERE (([ColumnIdentifiers].[Identifier] = @Parameter_p1) OR ([ColumnIdentifiers].[Column] = @Parameter_p2))";
        string actualSql = query.QueryText;
        Assert.Equal(expectedSql, actualSql);

        Assert.Equal(2, query.GetParameterCount());
        Assert.Equal(1, query.GetParameterValue<int>("@Parameter_p1"));
        Assert.Equal(2, query.GetParameterValue<int>("@Parameter_p2"));
    }

    [Fact]
    public void XOrTest()
    {
        Columns<ColumnIdentifiers> identifierColumn = new(nameof(ColumnIdentifiers.IdentifierName));
        Parameters parameter1 = new("p1", 1);
        Columns<ColumnIdentifiers> columnColumn = new(nameof(ColumnIdentifiers.ColumnName));
        Parameters parameter2 = new("p2", 2);
        Equal equal1 = new(identifierColumn, parameter1);
        Equal equal2 = new(columnColumn, parameter2);
        Xor xor = new(equal1, equal2);

        SqlQuery query = _generator.Select(null, null, xor, null, null);
        string expectedSql =
            "SELECT [ColumnIdentifiers].* FROM [ColumnIdentifiers] WHERE (([ColumnIdentifiers].[Identifier] = @Parameter_p1) ^ ([ColumnIdentifiers].[Column] = @Parameter_p2))";
        string actualSql = query.QueryText;
        Assert.Equal(expectedSql, actualSql);

        Assert.Equal(2, query.GetParameterCount());
        Assert.Equal(1, query.GetParameterValue<int>("@Parameter_p1"));
        Assert.Equal(2, query.GetParameterValue<int>("@Parameter_p2"));
    }

    [Fact] 
    public void ProcedureTest()
    {
        //Note: The context that determines if it is a procedure or a table is the generator function used.
        SqlQuery query = _generator.Procedure(_entity);

        string expectedSql = "[ColumnIdentifiers]";
        string actualSql = query.QueryText; 
        Assert.Equal(expectedSql, actualSql);

        Assert.Equal(5, query.GetParameterCount());
        Assert.Equal(1, query.GetParameterValue<int>("IdParameter"));
        Assert.Equal(2, query.GetParameterValue<int>("PropertyParameter"));
        Assert.Equal(3, query.GetParameterValue<int>("ColumnParameter"));
        Assert.Equal(4, query.GetParameterValue<int>("IdentifierParameter"));
        Assert.Equal(5, query.GetParameterValue<int>("IdentifierOverrideParameter"));
    }
}
