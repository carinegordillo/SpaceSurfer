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


            
            Assert.AreEqual("DELETE FROM Users WHERE Username = @Username", command.CommandText);
        }


        [TestMethod]
        public void CustomSqlCommandBuilder_CommandReset_Pass()
        {
            var builder = new CustomSqlCommandBuilder();
            var insertCommand = builder.BeginInsert("Users")
                                    .Columns(new[] { "Username", "Email" })
                                    .Values(new[] { "Username", "Email" })
                                    .Build();



            var updateCommand = builder.BeginUpdate("Users")
                                    .Set(new Dictionary<string, object> { { "Email", "new@example.com" } })
                                    .Where("Username = @Username")
                                    .Build();

            Assert.AreNotEqual(insertCommand.CommandText, updateCommand.CommandText);
        }



        [TestMethod]
        public void CustomSqlCommandBuilder_UpdateStatementCorrectParametersPassed_Pass()
        {
            var builder = new CustomSqlCommandBuilder();

            var parameters = new Dictionary<string, object>
            {
                { "Username", "john_doe" }, {"Age",12}
            };

            var command = builder.BeginUpdate("Users")
                                .Set(new Dictionary<string, object> { { "Email", "new@example.com" } })
                                .Where("Username = @Username AND Age > @Age")
                                .AddParameters(parameters)
                                .Build();



            Assert.AreEqual("john_doe", command.Parameters["@Username"].Value);

            Assert.AreEqual(12, command.Parameters["@Age"].Value);

        }

        [TestMethod]
        public void CustomSqlCommandBuilder_InsertStatementCorrectParametersPassed_Pass()
        {
            var builder = new CustomSqlCommandBuilder();

            var parameters = new Dictionary<string, object>
            {
                { "Username", "john_doe" }, {"Email","jd@gmail.com"}
            };

            var command = builder.BeginInsert("Users")
                                .Columns(new[] { "Username", "Email" })
                                .Values(new[] { "Username", "Email" })
                                .AddParameters(parameters)
                                .Build();
                                


            Assert.AreEqual("john_doe", command.Parameters["@Username"].Value);

            Assert.AreEqual("jd@gmail.com", command.Parameters["@Email"].Value);
        }

        [TestMethod]
        public void CustomSqlCommandBuilder_WhereStatementCorrectParametersPassed_Pass()
        {
            var builder = new CustomSqlCommandBuilder();

            var parameters = new Dictionary<string, object>
            {
                { "Username", "john_doe" }, {"Age",12}
            };

            var command = builder.BeginUpdate("Users")
                                .Set(new Dictionary<string, object> { { "Email", "new@example.com" } })
                                .Where("Username = @Username AND Age > @Age")
                                .AddParameters(parameters)
                                .Build();

            Assert.AreEqual("john_doe", command.Parameters["@Username"].Value);

            Assert.AreEqual(12, command.Parameters["@Age"].Value);
        }


        [TestMethod]
        public void CustomSqlCommandBuilder_ParametersOfVariousDataTypes_Pass()
        {
            // Arrange
            var builder = new CustomSqlCommandBuilder();
            var parameters = new Dictionary<string, object>
            {
                { "Username", "john_doe" },
                { "Age", 30 },
                { "IsActive", true },
                { "CreatedDate", new DateTime(2023, 1, 1) }
            };

            // Act
            var command = builder.BeginInsert("Users")
                                .Columns(new[] { "Username", "Age", "IsActive", "CreatedDate" })
                                .Values(new[] { "@Username", "@Age", "@IsActive", "@CreatedDate" })
                                .AddParameters(parameters)
                                .Build();

            // Assert
            Assert.AreEqual("john_doe", command.Parameters["@Username"].Value);
            Assert.AreEqual(30, command.Parameters["@Age"].Value);
            Assert.AreEqual(true, command.Parameters["@IsActive"].Value);
            Assert.AreEqual(new DateTime(2023, 1, 1), command.Parameters["@CreatedDate"].Value);
        }
        //new 
         

    }

}