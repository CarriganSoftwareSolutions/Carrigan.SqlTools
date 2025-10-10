using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.OrderByItems;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Tests.TestEntities;
using Carrigan.SqlTools.Tests.TestEntities.Attributes;

namespace Carrigan.SqlTools.Tests.AttributesTests;
public class ParameterAndColumnIdentifierTests
{
    //IGNORE SPELLING: tac
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

    private static readonly SqlGenerator<TableAndColumnIdentifiers> _tacGenerator = new();
    private static readonly TableAndColumnIdentifiers _tacEntity = new()
    {
        Id = 1,
        Column = 3
    };
    private static readonly TableAndColumnIdentifiers _tacModel = new()
    {
        Id = 6,
        Column = 8
    };

    private static readonly TableAndColumnIdentifiers _tacUpdateValues = new()
    {
        Column = 13
    };

    private static readonly IEnumerable<ColumnIdentifiers> _entities = [_entity, _model];
    private static readonly IEnumerable<TableAndColumnIdentifiers> _tacEntities = [_tacEntity, _tacModel];

    [Fact]
    public void DeleteTest()
    {
        SqlQuery query = _tacGenerator.Delete(_tacEntity);
        string actual = query.QueryText;
        string expected = "DELETE FROM [SomeSchema].[SomeTable] WHERE [SomeId] = @SomeIdParameter;";
        Assert.Equal(expected, actual);

        Assert.Equal(1, query.GetParameterCount());
        Assert.Equal(1, query.GetParameterValue<int>("SomeIdParameter"));
    }
    [Fact]
    public void DeleteAllTest()
    {
        SqlQuery query = _tacGenerator.DeleteAll();
        string actual = query.QueryText;
        string expected = "DELETE FROM [SomeSchema].[SomeTable];";
        Assert.Equal(expected, actual);

        Assert.Equal(0, query.GetParameterCount());
    }

    [Fact]
    public void DeleteIdPredicate()
    {
        ColumnValue<TableAndColumnIdentifiers> id = new(nameof(TableAndColumnIdentifiers.Id), 1);
        ColumnValue<TableAndColumnIdentifiers> column = new(nameof(TableAndColumnIdentifiers.Column), 3);
        And and = new(id, column);
        SqlQuery query = _tacGenerator.Delete(null, and);
        string actual = query.QueryText;
        string expected = "DELETE FROM [SomeSchema].[SomeTable] WHERE (([SomeSchema].[SomeTable].[SomeId] = @Parameter_SomeIdParameter) AND ([SomeSchema].[SomeTable].[SomeColumn] = @Parameter_SomeColumnParameter))";
        Assert.Equal(expected, actual);

        Assert.Equal(2, query.GetParameterCount());
        Assert.Equal(1, query.GetParameterValue<int>("@Parameter_SomeIdParameter"));
        Assert.Equal(3, query.GetParameterValue<int>("@Parameter_SomeColumnParameter"));
    }

    [Fact]
    public void InsertTest()
    {
        SqlQuery query = _tacGenerator.Insert(_tacEntity);
        string actual = query.QueryText;
        string expected = "INSERT INTO [SomeSchema].[SomeTable] ([SomeId], [SomeColumn]) VALUES (@SomeIdParameter, @SomeColumnParameter);";
        Assert.Equal(expected, actual);

        Assert.Equal(2, query.GetParameterCount());
        Assert.Equal(1, query.GetParameterValue<int>("SomeIdParameter"));
        Assert.Equal(3, query.GetParameterValue<int>("SomeColumnParameter"));
    }

    [Fact]
    public void InsertAutoIdTest()
    {
        SqlQuery query = _tacGenerator.InsertAutoId(_tacEntity);
        string actual = query.QueryText;
        string expected = SqlGenerator<TableAndColumnIdentifiers>.ModifyInsertQueryToReturnScalar("INSERT INTO [SomeSchema].[SomeTable] ([SomeColumn]) VALUES (SomeColumnParameter)");

        Assert.Equal(1, query.GetParameterCount());
        Assert.Equal(3, query.GetParameterValue<int>("SomeColumnParameter"));
    }

    [Fact]
    public void UpdateByIdTest()
    {
        SqlQuery query = _tacGenerator.UpdateById(_tacEntity);
        string actual = query.QueryText;
        string expected = "UPDATE [SomeSchema].[SomeTable] SET [SomeColumn] = @SomeColumnParameter WHERE [SomeId] = @SomeIdParameter;";
        Assert.Equal(expected, actual);

        Assert.Equal(2, query.GetParameterCount());
        Assert.Equal(1, query.GetParameterValue<int>("SomeIdParameter"));
        Assert.Equal(3, query.GetParameterValue<int>("SomeColumnParameter"));
    }

    [Fact]
    public void UpdateByIdsTest()
    {
        SqlQuery query = _tacGenerator.UpdateByIds(_tacUpdateValues, null, _tacEntities);
        string actual = query.QueryText;
        string expected = "UPDATE [SomeSchema].[SomeTable] SET [SomeSchema].[SomeTable].[SomeColumn] = @ParameterSet_SomeColumnParameter FROM [SomeSchema].[SomeTable] WHERE (([SomeSchema].[SomeTable].[SomeId] = @Parameter_0_R_SomeIdParameter) OR ([SomeSchema].[SomeTable].[SomeId] = @Parameter_1_R_SomeIdParameter))";
        Assert.Equal(expected, actual);

        Assert.Equal(3, query.GetParameterCount());
        Assert.Equal(1, query.GetParameterValue<int>("@Parameter_0_R_SomeIdParameter"));
        Assert.Equal(6, query.GetParameterValue<int>("@Parameter_1_R_SomeIdParameter"));
        Assert.Equal(13, query.GetParameterValue<int>("@ParameterSet_SomeColumnParameter"));
    }

    [Fact]
    public void JoinTest()
    {
        LeftJoin<JoinRightTable> join = new(new ColumnEqualsColumn<ColumnIdentifiers, JoinRightTable>(nameof(ColumnIdentifiers.Id), nameof(JoinRightTable.Id)));
        SqlQuery query = _generator.Delete(join.AsJoins<ColumnIdentifiers>(), null);
        string actual = query.QueryText;
        string expected = "DELETE FROM [ColumnIdentifiers] LEFT JOIN [Right] ON ([ColumnIdentifiers].[Id] = [Right].[Id])";
        Assert.Equal(expected, actual);

        Assert.Equal(0, query.GetParameterCount());
    }

    [Fact]
    public void LeftJoinTest()
    {
        LeftJoin<JoinRightTable> join = new(new ColumnEqualsColumn<ColumnIdentifiers, JoinRightTable>(nameof(ColumnIdentifiers.Id), nameof(JoinRightTable.Id)));
        SqlQuery query = _generator.Delete(join.AsJoins<ColumnIdentifiers>(), null);
        string actual = query.QueryText;
        string expected = "DELETE FROM [ColumnIdentifiers] LEFT JOIN [Right] ON ([ColumnIdentifiers].[Id] = [Right].[Id])";
        Assert.Equal(expected, actual);

        Assert.Equal(0, query.GetParameterCount());
    }

    [Fact]
    public void InnerJoinTest()
    {
        InnerJoin<JoinRightTable> join = new(new ColumnEqualsColumn<ColumnIdentifiers, JoinRightTable>(nameof(ColumnIdentifiers.Id), nameof(JoinRightTable.Id)));
        Joins<ColumnIdentifiers> joins = join.AsJoins<ColumnIdentifiers>();
        SqlQuery query = _generator.Delete(joins, null);
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
        Column<ColumnIdentifiers> identifierColumn = new(nameof(ColumnIdentifiers.IdentifierName));
        Parameter parameter1 = new(new ParameterTag(null, "p1", null), 1);
        Column<ColumnIdentifiers> columnColumn = new(nameof(ColumnIdentifiers.ColumnName));
        Parameter parameter2 = new(new ParameterTag(null, "p2", null), 2);
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
        Column<ColumnIdentifiers> identifierOverrideColumn = new(nameof(ColumnIdentifiers.IdentifierOverrideName));
        Parameter parameter =new(new ParameterTag(null, "p1", null), 1);
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
        ColumnValue<ColumnIdentifiers> whereIdEquals = new(nameof(ColumnIdentifiers.Id), 1);
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
        Column<ColumnIdentifiers> column = new(nameof(ColumnIdentifiers.IdentifierName));
        Parameter parameter = new("p1", 1);
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
        Column<ColumnIdentifiers> column = new(nameof(ColumnIdentifiers.IdentifierName));
        Parameter parameter = new("p1", 1);
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
        Column<ColumnIdentifiers> column = new(nameof(ColumnIdentifiers.IdentifierName));
        Parameter parameter = new("p1", 1);
        GreaterThanEqual op = new(column, parameter);

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
        Column<ColumnIdentifiers> column = new(nameof(ColumnIdentifiers.IdentifierName));
        Parameter parameter = new("p1", 1);
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
        Column<ColumnIdentifiers> column = new(nameof(ColumnIdentifiers.IdentifierName));
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
        Column<ColumnIdentifiers> column = new(nameof(ColumnIdentifiers.IdentifierName));
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
        Column<ColumnIdentifiers> column = new(nameof(ColumnIdentifiers.IdentifierName));
        Parameter parameter = new("p1", 1);
        LessThanEqual op = new(column, parameter);

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
        Column<ColumnIdentifiers> column = new(nameof(ColumnIdentifiers.IdentifierName));
        Parameter parameter = new("p1", 1);
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
        Column<ColumnIdentifiers> column = new(nameof(ColumnIdentifiers.IdentifierName));
        Parameter parameter = new("p1", 1);
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
        Column<ColumnIdentifiers> column = new(nameof(ColumnIdentifiers.IdentifierName));
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
        Column<ColumnIdentifiers> identifierColumn = new(nameof(ColumnIdentifiers.IdentifierName));
        Parameter parameter1 = new("p1", 1);
        Column<ColumnIdentifiers> columnColumn = new(nameof(ColumnIdentifiers.ColumnName));
        Parameter parameter2 = new("p2", 2);
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
        Column<ColumnIdentifiers> identifierColumn = new(nameof(ColumnIdentifiers.IdentifierName));
        Parameter parameter1 = new("p1", 1);
        Column<ColumnIdentifiers> columnColumn = new(nameof(ColumnIdentifiers.ColumnName));
        Parameter parameter2 = new("p2", 2);
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
