using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.OffsetNexts;
using Carrigan.SqlTools.OrderByItems;
using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities; //this is where Customer and Order are defined.
using System.Text;


namespace Carrigan.SqlTools.Tests.ExamplesAsUnitTests;
public class PredicateExamples
{
    private SqlGenerator<Customer> customerGenerator = new();
    private SqlGenerator<Order> orderGenerator = new();


    [Fact]
    public void SelectAnd()
    {
        Parameters parameterName = new("Name", "Hank");
        Columns<Customer> columnName = new(nameof(Customer.Name));
        Equal equalName = new(columnName, parameterName);

        Parameters parameterEmail = new ("Email", "Hank@example.com");
        Columns<Customer> columnEmail = new(nameof(Customer.Email));
        Equal equalEmail = new(columnEmail, parameterEmail);


        Parameters parameterPhone = new("Phone", ("+1(555)555-5555"));
        Columns<Customer> columnPhone = new(nameof(Customer.Phone));
        Equal equalPhone = new(columnPhone, parameterPhone);

        And and = new And(equalName, equalEmail, equalPhone);

        SqlQuery query = customerGenerator.Select(null, and, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE (([Customer].[Name] = @Parameter_Name) AND ([Customer].[Email] = @Parameter_Email) AND ([Customer].[Phone] = @Parameter_Phone))", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Equal(3, query.Parameters.Count);

        Assert.Equal("Hank", (string)query.Parameters.Single(param => param.Key == "@Parameter_Name").Value);
        Assert.Equal("Hank@example.com", (string)query.Parameters.Single(param => param.Key == "@Parameter_Email").Value);
        Assert.Equal("+1(555)555-5555", (string)query.Parameters.Single(param => param.Key == "@Parameter_Phone").Value);
    }
}
