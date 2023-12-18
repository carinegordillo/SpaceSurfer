
using SS.Backend.SharedNamespace;
using SS.Backend.UserManagement;

namespace SS.Backend.Tests.UserManagement;

[TestClass]
public class AccountRecoveryTests
{
    private AccountRecovery _accountRecovery;

    [TestInitialize]
    public void Setup()
    {
        // Initialize AccountRecovery with real dependencies
        // This assumes that you can construct AccountStatusModifier without mocks
        AccountStatusModifier accountStatusModifier = new AccountStatusModifier();
        _accountRecovery = new AccountRecovery(accountStatusModifier);
    }

    [TestMethod]
    public async Task SendRecoveryRequest_ShouldReturnExpectedResult()
    {
        // Arrange
        var userHash = "testUsername";

        // Act
        var response = await _accountRecovery.sendRecoveryRequest(userHash);

        // Assert
        // Your assertions here will depend on the actual implementation and side effects of SendRecoveryRequest
        // For example:
        Assert.IsFalse(response.HasError);
        // Assert.IsTrue(response.Success); // Uncomment if applicable
        // Assert.AreEqual("Expected Message", response.Message); // Uncomment and modify as needed
    }

    [TestMethod]
    public async Task RecoverAccount_ShouldReturnExpectedResult()
    {
        // Arrange
        var userHash = "testUsername";
        bool adminDecision = true; // or false, depending on the test case

        // Act
        var response = await _accountRecovery.RecoverAccount(userHash, adminDecision);

        // Assert
        Assert.IsFalse(response.HasError);
    }

    [TestMethod]
    public async Task SendRecoveryRequest_InvalidUserHash_ShouldFail()
    {
        // Arrange
        var userHash = "invalidUserHash";

        // Act
        var response = await _accountRecovery.sendRecoveryRequest(userHash);

        // Assert
        Assert.IsTrue(response.HasError);
     
    }

    [TestMethod]
    public async Task SendRecoveryRequest_AlreadyActiveAccount_ShouldIndicateUnnecessaryAction()
    {
        // Arrange

         var userHash = "testUsername2";

        var response = await _accountRecovery.sendRecoveryRequest(userHash);
    

        // Act
        response = await _accountRecovery.sendRecoveryRequest(userHash);

        // Assert
        Assert.IsTrue(response.RowsAffected == 0); 
    }
 
    [TestMethod]
    public async Task RecoverAccount_NonExistentUser_ShouldFail()
    {
        // Arrange
        var userHash = "nonExistentUserHash";
        bool adminDecision = true; // Admin decision doesn't matter in this case

        // Act
        var response = await _accountRecovery.RecoverAccount(userHash, adminDecision);

        // Assert
        Assert.IsTrue(response.HasError);
        
    }

    [TestMethod]
    public async Task RecoverAccount_WithoutAdminApproval_ShouldFail()
    {
        // Arrange
        var userHash = "someUserHash";
        bool adminDecision = false; // Admin decision is negative

        // Act
        var response = await _accountRecovery.RecoverAccount(userHash, adminDecision);

        // Assert
        Assert.IsTrue(response.HasError);
        Assert.AreEqual("Recovery request denied by admin.", response.ErrorMessage); 
    }

    [TestMethod]
    public async Task RecoverAccount_WithPendingRecoveryRequest_ShouldHandleGracefully()
    {
        // Arrange
        var userHash = "userWithPendingRequest"; // Use a userHash that has a pending recovery request
        bool adminDecision = true; // Assuming admin approval is required

        // Send an initial recovery request (assuming this sets the recovery request as pending)
        await _accountRecovery.sendRecoveryRequest(userHash);

        // Act
        // Now attempt to recover the account again
        var response = await _accountRecovery.RecoverAccount(userHash, adminDecision);

        // Assert
        // The exact assertion will depend on how your system is expected to behave in this scenario
        Assert.IsFalse(response.HasError);
    }

    [TestMethod]
    public async Task SendRecoveryRequest_MultipleConcurrentRequests_ShouldHandleConcurrently()
    {
        // Arrange
        var userHash = "userForConcurrentTesting";

        // Act
        var task1 = _accountRecovery.sendRecoveryRequest(userHash);
        var task2 = _accountRecovery.sendRecoveryRequest(userHash);
        var task3 = _accountRecovery.sendRecoveryRequest(userHash);

        // Await all tasks to complete
        var responses = await Task.WhenAll(task1, task2, task3);

        // Assert
        // The assertions depend on how your system handles concurrent requests
        foreach (var response in responses)
        {
            Assert.IsFalse(response.HasError);
            
        }
    }









}
