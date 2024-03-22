
using SS.Backend.SharedNamespace;
using SS.Backend.UserManagement;
using System.Data.SqlClient;


namespace SS.Backend.Tests.UserManagement;

[TestClass]
public class ProfileModification
{

    [TestMethod]
    public async Task ProfileModification_ModifyFirstName_Pass()
    {
        Response response = new Response();
        ProfileModifier accountModifier = new ProfileModifier();

        response = await accountModifier.ModifyFirstName("testUsername2","Carine");
        Assert.IsFalse(response.HasError);
        Assert.IsTrue(response.RowsAffected>0);

    }

    [TestMethod]
    public async Task ProfileModification_ModifyLastName_Pass()
    {
        Response response = new Response();
        ProfileModifier accountModifier = new ProfileModifier();

        response = await accountModifier.ModifyFirstName("testUsername3","Gordillo");
        Assert.IsFalse(response.HasError);
        Assert.IsTrue(response.RowsAffected>0);

    }

    [TestMethod]
    public async Task ProfileModification_ModifyBackupEmail_Pass()
    {
        Response response = new Response();
        ProfileModifier accountModifier = new ProfileModifier();

        response = await accountModifier.ModifyBackupEmail("testUsername2","carine@gmail.com");
        Assert.IsFalse(response.HasError);
        Assert.IsTrue(response.RowsAffected>0);

    }

    [TestMethod]
    public async Task ModifyFirstName_InvalidUserHash_ShouldFail()
    {
        // Arrange
        ProfileModifier accountModifier = new ProfileModifier();
        var invalidUserHash = "invalidUserHash";

        // Act
        var response = await accountModifier.ModifyFirstName(invalidUserHash, "NewFirstName");

        // Assert
        Assert.IsTrue(response.HasError);
    }
    
    [TestMethod]
    public async Task ModifyFirstName_NullOrEmptyName_ShouldFail()
    {
        // Arrange
        ProfileModifier accountModifier = new ProfileModifier();
        var userHash = "validUserHash"; 

        // Test with null
        var responseNull = await accountModifier.ModifyFirstName(userHash, null);

        // Test with empty string
        var responseEmpty = await accountModifier.ModifyFirstName(userHash, string.Empty);

        // Assert
        Assert.IsTrue(responseNull.HasError);
        Assert.IsTrue(responseEmpty.HasError);
    }

    [TestMethod]
    public async Task ModifyProfile_ConcurrentModifications_ShouldPass()
    {
        // Arrange
        ProfileModifier accountModifier = new ProfileModifier();
        var userHash = "hashedUser4"; // Replace with a valid user hash

        // Act
        var task1 = accountModifier.ModifyFirstName(userHash, "NewFirstName");
        var task2 = accountModifier.ModifyLastName(userHash, "NewLastName");
        var task3 = accountModifier.ModifyBackupEmail(userHash, "newemail@example.com");

        // Await all tasks to complete
        var responses = await Task.WhenAll(task1, task2, task3);

        // Assert
        foreach (var response in responses)
        {
            Assert.IsFalse(response.HasError);
        }
    }
    
        [TestMethod]
        public async Task ModifyField_NullOrEmptyUserHash_ShouldFail()
        {
            // Arrange
            ProfileModifier accountModifier = new ProfileModifier();
            var newValue = "NewValue"; 

            // Test with null
            var responseNull = await accountModifier.ModifyFirstName(null, newValue);

            // Test with empty string
            var responseEmpty = await accountModifier.ModifyFirstName(string.Empty, newValue);

            // Assert
            Assert.IsTrue(responseNull.HasError);
            Assert.IsTrue(responseEmpty.HasError);
        }

        //create an email formatter for valid emails 





    

}
