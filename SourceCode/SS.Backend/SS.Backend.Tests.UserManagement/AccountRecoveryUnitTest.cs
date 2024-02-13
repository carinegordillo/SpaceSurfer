
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
        AccountRecoveryModifier accountRecoveryModifier = new AccountRecoveryModifier();
        _accountRecovery = new AccountRecovery(accountRecoveryModifier);
    }

    [TestMethod]
    public async Task SendInitialRecoveryRequest_Pass()
    {
        // Arrange
        var userHash = "testUsername";

        // Act
        var response = await _accountRecovery.sendRecoveryRequest(userHash);

        // Assert

        Assert.IsFalse(response.HasError);

    }

    [TestMethod]
    public async Task RecoverAccount_AdminDecision_True_ShouldPass()
    {
        // Arrange
        var userHash = "testUsername";
        bool adminDecision = true; 

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
        Console.Write(response.ErrorMessage);
        Assert.IsTrue(response.HasError);
     
    }

    [TestMethod]
    public async Task SendRecoveryRequest_AlreadyActiveAccount_ShouldPass()
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
        bool adminDecision = true; 

        // Act
        var response = await _accountRecovery.RecoverAccount(userHash, adminDecision);

        // Assert
        Console.Write(response.ErrorMessage);
        Assert.IsTrue(response.HasError);
        
    }

    [TestMethod]
    public async Task RecoverAccount_WithoutAdminApproval_ShouldFail()
    {
        // Arrange
        var userHash = "someUserHash";
        bool adminDecision = false; 

        // Act
        var response = await _accountRecovery.RecoverAccount(userHash, adminDecision);

        // Assert
        Assert.IsTrue(response.HasError);
        Assert.AreEqual("Recovery request denied by admin.", response.ErrorMessage); 
    }

    [TestMethod]
    public async Task RecoverAccount_WithPendingRecoveryRequest_ShouldPass()
    {
        // Arrange
        var userHash = "userWithPendingRequest"; 
        bool adminDecision = true; 

        await _accountRecovery.sendRecoveryRequest(userHash);

        // Act

        var response = await _accountRecovery.RecoverAccount(userHash, adminDecision);

        // Assert
  
        Assert.IsFalse(response.HasError);
    }

    [TestMethod]
    public async Task SendRecoveryRequest_MultipleConcurrentRequests_ShouldPass()
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
        
        foreach (var response in responses)
        {
            Assert.IsFalse(response.HasError);
            
        }
    }

    [TestMethod]
    public async Task CreateRecoveryRequest_ValidUserHash_ShouldSucceed()
    {
        // Arrange
        var userHash = "userHash1";
        string additionalInfo = "Need to recover account due to forgotten password.";

        // Actx
        var response = await _accountRecovery.createRecoveryRequest(userHash, additionalInfo);

        // Assert
        Console.Write(response.ErrorMessage);
        Assert.IsFalse(response.HasError);
    }









}
