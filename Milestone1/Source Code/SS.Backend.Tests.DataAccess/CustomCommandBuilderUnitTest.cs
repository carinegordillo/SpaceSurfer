using SS.Backend.DataAccess;
using System.Data.SqlClient;

namespace SS.Backend.Tests.DataAccess
{
    [TestClass]
    public class CustomSqlCommandBuilderUnitTest
    {

        [TestMethod]
        public void CustomSqlCommandBuilder_Correct_InsertCommand_Pass()
        {
            var builder = new CustomSqlCommandBuilder();
            var command = builder.BeginInsert("Users")
                                .Columns(new[] { "Username", "Email" })
                                .Values(new[] { "Username", "Email" })
                                .Build();

            Assert.AreEqual("INSERT INTO Users (Username, Email) VALUES (@Username, @Email)", command.CommandText);
        }

        [TestMethod]
        public void CustomSqlCommandBuilder_Correct_UpdateCommand_Pass()
        {
            var builder = new CustomSqlCommandBuilder();
            var command = builder.BeginUpdate("Users")
                                .Set(new Dictionary<string, object> { { "Email", "new@example.com" } })
                                .Where("Username = @Username")
                                .Build();

            Assert.AreEqual("UPDATE Users SET Email = @Email WHERE Username = @Username", command.CommandText);
        }

        [TestMethod]
        public void CustomSqlCommandBuilder_Correct_DeleteCommand_Pass()
        {
            var parameters = new Dictionary<string, object>
            {
                { "@Username", "john_doe" },
            };

            var builder = new CustomSqlCommandBuilder();
            var command = builder.BeginDelete("Users")
                                .Where("Username = @Username")
                                .AddParameters(parameters)
                                .Build();

            Console.WriteLine(command.CommandText);

            Assert.AreEqual("DELETE FROM Users WHERE Username = john_doe", command.CommandText);
        }

        [TestMethod]
        public void CustomSqlCommandBuilder_CorrectParametersPassed_Pass()
        {
            var builder = new CustomSqlCommandBuilder();
            var parameters = new Dictionary<string, object>
            {
                { "Username", "john_doe" },
                { "Email", "john@example.com" }
            };

            var command = builder.BeginInsert("Users")
                                .Columns(new[] { "Username", "Email" })
                                .Values(new[] { "Username", "Email" })
                                .AddParameters(parameters)
                                .Build();
            foreach (SqlParameter param in command.Parameters)
            {
                Console.WriteLine($"{param.ParameterName}, {param.Value}");
            }

            // Correct assertions
            Assert.IsTrue(command.Parameters.Contains("@Username"));
            Assert.AreEqual("john_doe", command.Parameters["@Username"].Value);

            Assert.IsTrue(command.Parameters.Contains("@Email"));
            Assert.AreEqual("john@example.com", command.Parameters["@Email"].Value);
        }

        [TestMethod]
        public void CustomSqlCommandBuilder_CommandReset_Pass()
        {
            var builder = new CustomSqlCommandBuilder();
            var insertCommand = builder.BeginInsert("Users")
                                    .Columns(new[] { "Username", "Email" })
                                    .Values(new[] { "Username", "Email" })
                                    .Build();

            Console.WriteLine(insertCommand.CommandText);

            var updateCommand = builder.BeginUpdate("Users")
                                    .Set(new Dictionary<string, object> { { "Email", "new@example.com" } })
                                    .Where("Username = @Username")
                                    .Build();
            Console.WriteLine(updateCommand.CommandText);
            Console.WriteLine(insertCommand.CommandText);

            Assert.AreNotEqual(insertCommand.CommandText, updateCommand.CommandText);
        }

    }

}