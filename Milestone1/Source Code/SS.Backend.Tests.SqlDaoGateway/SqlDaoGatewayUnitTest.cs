using SS.Backend.DataGateway;
using SS.Backend.SharedNamespace;
using System.Data.SqlClient;
using System.Diagnostics;


namespace SS.Backend.Tests.DataGateway;
[TestClass]
public class SqlDaoGatewayUnitTest
{
    [TestMethod]
    public async Task SqlDaoGateWay_Successful_Paramter_and_Text_Seperator_Pass()
    {
        // Arrange
        SqlDaoGateway gateway = new SqlDaoGateway(); 
        var command = new SqlCommand("SELECT * FROM Table WHERE ID = @id");
        command.Parameters.AddWithValue("@id", 1);

        // Act
        string commandText = gateway.GetSqlCommandText(command);
        string commandParameters = gateway.GetSqlCommandParameters(command);

        // Assert
        Assert.AreEqual("SELECT * FROM Table WHERE ID = @id", commandText);
        Assert.IsTrue(commandParameters.Contains("@id = 1"));
    }
}