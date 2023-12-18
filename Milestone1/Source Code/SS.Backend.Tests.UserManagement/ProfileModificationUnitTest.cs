
using SS.Backend.SharedNamespace;
using SS.Backend.UserManagement;
using System.Data.SqlClient;


namespace SS.Backend.Tests.UserManagement;

[TestClass]
public class ProfileModification
{

    [TestMethod]
    public async Task ProfileModification_ModifyFirstName()
    {
        Response response = new Response();
        ProfileModifier accountModifier = new ProfileModifier();

        response = await accountModifier.ModifyFirstName("testUsername2","Carine");
        Assert.IsFalse(response.HasError);
        Assert.IsTrue(response.RowsAffected>0);

    }

    [TestMethod]
    public async Task ProfileModification_ModifyLastName()
    {
        Response response = new Response();
        ProfileModifier accountModifier = new ProfileModifier();

        response = await accountModifier.ModifyFirstName("testUsername3","Gordillo");
        Assert.IsFalse(response.HasError);
        Assert.IsTrue(response.RowsAffected>0);

    }

    [TestMethod]
    public async Task ProfileModification_ModifyBackupEmail()
    {
        Response response = new Response();
        ProfileModifier accountModifier = new ProfileModifier();

        response = await accountModifier.ModifyBackupEmail("testUsername2","carine@gmail.com");
        Assert.IsFalse(response.HasError);
        Assert.IsTrue(response.RowsAffected>0);

    }

    

    
}