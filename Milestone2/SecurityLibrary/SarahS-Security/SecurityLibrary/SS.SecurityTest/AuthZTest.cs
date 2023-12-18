using SS.Security;

namespace SS.SecurityTest;

[TestClass]
public class AuthZTest
{
    [TestMethod]
    // isAuthorize - Success (true)
    public void IsAuthorize_Pass()
    {
        // Arrange
        var authService = new SSAuthService();
        var currentPrincipal = new SSPrincipal
        {
            UserIdentity = "John",
            Claims = new Dictionary<string, string>
            {
                {"Role", "Admin"},
                {"Department", "IT"}
            },
            SecurityContext = "custom"
        };

        var requiredClaims = new Dictionary<string, string>
        {
            {"Role", "Admin"},
            {"Department", "IT"}
        };

        // Act
        bool result = authService.IsAuthorize(currentPrincipal, requiredClaims);

        // Assert
        Assert.IsTrue(result, "The user has all the required claims, so IsAuthorize should return true.");
    }

    //isAuthorize - Fail (false)


    [TestMethod]
    //HasClaim - Success (true)
    public void HasClaim_Pass()
    {
        // arrange
        var valid_user = new SSPrincipal
        {
            UserIdentity = "John",
            Claims = new Dictionary<string, string>
            {
                {"Role", "Admin"},
                {"Department", "IT"},
            },
            SecurityContext = "custom",
        };

        // act
        bool result = valid_user.HasClaim("Role", "Admin");

        //assert
        Assert.IsTrue(result, $"Expected user '{valid_user.UserIdentity}' to have the 'Role' claim with value 'Admin'.");
    }
    
    //HasClaim - Fail (false)
}