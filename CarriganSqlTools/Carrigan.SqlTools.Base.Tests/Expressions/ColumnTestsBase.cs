using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public abstract class ColumnTestsBase<modelT> where modelT : class
{
    protected abstract ISqlDialects Dialect { get; }

    protected abstract string? SchemaName { get; }

    protected abstract string TableName { get; }


    protected KeyValuePair<string, ColumnName> NewKvp(string propertyName, string ColumnName) =>
        new(propertyName, new ColumnName(ColumnName));

    protected KeyValuePair<string, ColumnName> NewKvp(string propertyName) =>
        new(propertyName, new ColumnName(propertyName));

    internal Dictionary<string, ColumnTag> ExpectedPropertyColumnTag =>
    new
    (
        ExpectedPropertyColumnName.
            Keys.
            Select(key => new KeyValuePair<string, ColumnTag>(key, new ColumnTag(new TableTag(SchemaName, TableName), ExpectedPropertyColumnName[key])))
    );

    internal abstract Dictionary<string, ColumnName> ExpectedPropertyColumnName { get; }

    protected virtual IEnumerable<string> NotMappedProperties => [];

    protected TableTag ExpectedTableTag =>
        new(SchemaName, TableName);

    protected TableName ExpectedTableName =>
        new(TableName);

    protected SchemaName? ExpectedSchemaName =>
        IdentifierTypes.SchemaName.New(SchemaName);

    protected abstract string ExpectSqlFragment(string expectedColumnName);

    protected ColumnTestsBase()
    {
    }

    protected abstract void ValidateColumnSqlFragments(string propertyName);

    protected abstract void ValidateSqlFragment(string propertyName);

    protected abstract void ValidateExpectedPropertyColumnTag(string propertyName);

    protected abstract void ValidateExpectedPropertyColumnName(string propertyName);

    protected abstract void ValidateExpectedPropertyTableTag(string propertyName);

    protected abstract void ValidateExpectedPropertyTableName(string propertyName);

    protected abstract void ValidateExpectedPropertySchemaName(string propertyName);

    protected abstract void ValidateNoDescendantParameters(string propertyName);

    protected abstract void ValidateNoDescendantColumns(string propertyName);

    protected abstract void ValidateNotMapped();

    protected void RunValidationMethod(Action<string> action)
    {
        foreach (string propertyName in ExpectedPropertyColumnTag.Keys)
        {
            action(propertyName);
        }
    }

    [Fact]
    public void Run_ValidateSqlFragment() =>
        RunValidationMethod(ValidateSqlFragment);
}
