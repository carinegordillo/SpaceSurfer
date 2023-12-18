using SS.Security;

namespace SS.Backend.SecurityTest;

[TestClass]
public class AuthZTest
{
    [TestMethod]
    //IsAuthorize - Success (true)
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
        bool await result = authService.IsAuthorize(currentPrincipal, requiredClaims);

        // Assert
        Assert.IsTrue(result, "The user has all the required claims, so IsAuthorize should return true.");
    }

    [TestMethod]
    //IsAuthorize - Failed Required Claims(false)
    public void IsAuthorize_FailedRequiredClaims()
    {
        // Arrange
        var authService = new SSAuthService();

        var currentPrincipal = new SSPrincipal
        {
            UserIdentity = "Jane",
            Claims = new Dictionary<string, string>
            {
                {"Role", "GenUser"}
            }
        };

        var requiredClaims = new Dictionary<string, string>
        {
            {"Role", "Admin"}
        };

        // Act
        bool result = authService.IsAuthorize(currentPrincipal, requiredClaims);

        // Assert
        Assert.IsFalse(result, "The user is missing a required claim, so IsAuthorize should return false.");
    }

    [TestMethod]
    //IsAuthorize - Failed Null Claims (false)
    public void IsAuthorize_FailedNullClaims()
    {
    // Arrange
        var authService = new SSAuthService();

        var currentPrincipal = new SSPrincipal
        {
            UserIdentity = "Jimmy",
            Claims = null
        };

        var requiredClaims = new Dictionary<string, string>
        {
            {"Role", "Admin"}
        };

        // Act
        bool result = authService.IsAuthorize(currentPrincipal, requiredClaims);

        // Assert
        Assert.IsFalse(result, "The user claims are null, so IsAuthorize should return false.");
    
}