using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using System.Data.SqlClient;
using SS.Backend.Services.AccountCreationService;
using System.Data.SqlClient;
using System.Diagnostics;

namespace SS.Backend.Tests.AccountCreationTest
{
    [TestClass]
    public class AccountCreationTests
    {
        private async Task CleanupTestData()
        {
            var SAUser = Credential.CreateSAUser();
            var connectionString = string.Format(@"Data Source=localhost\SpaceSurfer;Initial Catalog=SS_Server;User Id={0};Password={1};", SAUser.user, SAUser.pass);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync().ConfigureAwait(false);

                    string sql = $"DELETE FROM dbo.Logs WHERE [Username] = 'test@email'";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during test cleanup: {ex}");
            }
        }

        [TestMethod]
        public async Task ReadPepperFileContents_Success()
        {
            string filePath = "C:/Users/kayka/Downloads/pepper.txt";
            SealedPepperDAO pepperDao = new SealedPepperDAO(filePath);
            string pepper = await pepperDao.ReadPepperAsync();
            Console.WriteLine($"Pepper content: {pepper}");
            Assert.IsFalse(string.IsNullOrEmpty(pepper), "The pepper file should not be empty.");
        }
        

        [TestMethod]
        public async Task CreateUserAccount_Success()
        {
            // AccountCreation accountcreation = new AccountCreation(SqlDAO sqlDao, ICustomSqlCommandBuilder commandBuilder);
            UserInfo userInfo = new UserInfo();
            UserPepper userPepper = new UserPepper();
            AccountCreation accountcreation = new AccountCreation(userInfo);
            Hashing hashing = new Hashing();
            Stopwatch timer = new Stopwatch();

            Credential removeMeLater = Credential.CreateSAUser();
            SealedSqlDAO SQLDao = new SealedSqlDAO(removeMeLater);
            var builder = new CustomSqlCommandBuilder();

            SealedPepperDAO pepperDao = new SealedPepperDAO("C:/Users/kayka/Downloads/pepper.txt");
            string pepper = await pepperDao.ReadPepperAsync();

            //username must be unique in database
            var validUserInfo = new UserInfo{
                username = "thidkanymoreG@example.com",
                dob = new DateTime(1990, 1, 1),
                firstname = "Jane",
                lastname = "Doe", 
                role = 5,
                status = "yes", 
                backupEmail = "test@backup.com"
            };

      
            var validPepper = new UserPepper{
                hashedUsername = hashing.HashData(validUserInfo.username, pepper)
            };
    
            var userAccount_success_parameters = new Dictionary<string, object>
            {
                { "username", validUserInfo.username},
                {"birthDate", validUserInfo.dob}   
            };

            var userProfile_success_parameters = new Dictionary<string, object>
            {
                {"hashedUsername", validPepper.hashedUsername},
                { "FirstName", validUserInfo.firstname},
                { "LastName", validUserInfo.lastname}, 
                {"backupEmail", validUserInfo.backupEmail},
                {"appRole", validUserInfo.role}, 
            };
            
            var activeAccount_success_parameters = new Dictionary<string, object>
            {
                {"hashedUsername", validPepper.hashedUsername},
                {"isActive", validUserInfo.status} 
            };

            var hashedAccount_success_parameters = new Dictionary<string, object>
                {
                    {"hashedUsername", validPepper.hashedUsername},
                    {"username", validUserInfo.username},
                };

            var tableData = new Dictionary<string, Dictionary<string, object>>
            {
                { "userAccount", userAccount_success_parameters },
                { "userProfile", userProfile_success_parameters },
                { "activeAccount", activeAccount_success_parameters}, 
                {"userHash", hashedAccount_success_parameters}
            };

            timer.Start();
            var response = await accountcreation.CreateUserAccount(validPepper, validUserInfo, tableData);
            timer.Stop();

            Assert.IsFalse(response.HasError, response.ErrorMessage);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 5000);
            await CleanupTestData().ConfigureAwait(false);
          
        }


        [TestMethod]
        public async Task CreateUserAccount_InvalidUsername()
        {
            // AccountCreation accountcreation = new AccountCreation(SqlDAO sqlDao, ICustomSqlCommandBuilder commandBuilder);
            UserInfo userInfo = new UserInfo();
            UserPepper userPepper = new UserPepper();
            AccountCreation accountcreation = new AccountCreation(userInfo);
            Hashing hashing = new Hashing();
            Stopwatch timer = new Stopwatch();

            Credential removeMeLater = Credential.CreateSAUser();
            SealedSqlDAO SQLDao = new SealedSqlDAO(removeMeLater);
            var builder = new CustomSqlCommandBuilder();

            //username already exists in db so will throw an error 
            var validUserInfo = new UserInfo{
                username = "newtest@example.com",
                dob = new DateTime(1990, 1, 1),
                firstname = "Jane",
                lastname = "Doe", 
                role = 5,
                status = "yes", 
                backupEmail = "test@backup.com"
            };

    
            SealedPepperDAO pepperDao = new SealedPepperDAO("C:/Users/kayka/Downloads/pepper.txt");
            string pepper = await pepperDao.ReadPepperAsync();

            var validPepper = new UserPepper{
                hashedUsername = hashing.HashData(validUserInfo.username, pepper)
            };
    
            var userAccount_success_parameters = new Dictionary<string, object>
            {
                { "username", validUserInfo.username},
                {"birthDate", validUserInfo.dob}   
            };

            var userProfile_success_parameters = new Dictionary<string, object>
            {
                {"hashedUsername", validPepper.hashedUsername},
                { "FirstName", validUserInfo.firstname},
                { "LastName", validUserInfo.lastname}, 
                {"backupEmail", validUserInfo.backupEmail},
                {"appRole", validUserInfo.role}, 
            };
            
            var activeAccount_success_parameters = new Dictionary<string, object>
            {
                {"hashedUsername", validPepper.hashedUsername},
                {"isActive", validUserInfo.status} 
            };

            var hashedAccount_success_parameters = new Dictionary<string, object>
                {
                    {"hashedUsername", validPepper.hashedUsername},
                    {"username", validUserInfo.username},
                };

            var tableData = new Dictionary<string, Dictionary<string, object>>
            {
                { "userAccount", userAccount_success_parameters },
                { "userProfile", userProfile_success_parameters },
                { "activeAccount", activeAccount_success_parameters}, 
                {"userHash", hashedAccount_success_parameters}
            };


            timer.Start();
            var response = await accountcreation.CreateUserAccount(validPepper, validUserInfo, tableData);
            timer.Stop();

            Assert.IsTrue(response.HasError, response.ErrorMessage);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 5000);
            await CleanupTestData().ConfigureAwait(false);
        }


        [TestMethod]
        public async Task CreateUserAccount_InvalidFirstName()
        {
            // AccountCreation accountcreation = new AccountCreation(SqlDAO sqlDao, ICustomSqlCommandBuilder commandBuilder);
            UserInfo userInfo = new UserInfo();
            UserPepper userPepper = new UserPepper();
            AccountCreation accountcreation = new AccountCreation(userInfo);
            Hashing hashing = new Hashing();
            Stopwatch timer = new Stopwatch();

            Credential removeMeLater = Credential.CreateSAUser();
            SealedSqlDAO SQLDao = new SealedSqlDAO(removeMeLater);
            var builder = new CustomSqlCommandBuilder();

            //first name is null so will throw an error 
            var validUserInfo = new UserInfo{
                username = "now2323trythis@example.com",
                dob = new DateTime(1990, 1, 1),
                firstname = "",
                lastname = "Doe", 
                role = 5,
                status = "yes", 
                backupEmail = "test@backup.com"
            };

            SealedPepperDAO pepperDao = new SealedPepperDAO("C:/Users/kayka/Downloads/pepper.txt");
            string pepper = await pepperDao.ReadPepperAsync();

            var validPepper = new UserPepper{
                hashedUsername = hashing.HashData(validUserInfo.username, pepper)
            };
    
            var userAccount_success_parameters = new Dictionary<string, object>
            {
                { "username", validUserInfo.username},
                {"birthDate", validUserInfo.dob}   
            };

            var userProfile_success_parameters = new Dictionary<string, object>
            {
                {"hashedUsername", validPepper.hashedUsername},
                { "FirstName", validUserInfo.firstname},
                { "LastName", validUserInfo.lastname}, 
                {"backupEmail", validUserInfo.backupEmail},
                {"appRole", validUserInfo.role}, 
            };
            
            var activeAccount_success_parameters = new Dictionary<string, object>
            {
                {"hashedUsername", validPepper.hashedUsername},
                {"isActive", validUserInfo.status} 
            };

            var hashedAccount_success_parameters = new Dictionary<string, object>
                {
                    {"hashedUsername", validPepper.hashedUsername},
                    {"username", validUserInfo.username},
                };

            var tableData = new Dictionary<string, Dictionary<string, object>>
            {
                { "userAccount", userAccount_success_parameters },
                { "userProfile", userProfile_success_parameters },
                { "activeAccount", activeAccount_success_parameters}, 
                {"userHash", hashedAccount_success_parameters}
            };


            timer.Start();
            var response = await accountcreation.CreateUserAccount(validPepper, validUserInfo, tableData);
            timer.Stop();

            Assert.IsTrue(response.HasError, response.ErrorMessage);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 5000);
            await CleanupTestData().ConfigureAwait(false);
          
        }


        [TestMethod]
        public async Task CreateUserAccount_InvalidLastName()
        {
            // AccountCreation accountcreation = new AccountCreation(SqlDAO sqlDao, ICustomSqlCommandBuilder commandBuilder);
            UserInfo userInfo = new UserInfo();
            UserPepper userPepper = new UserPepper();
            AccountCreation accountcreation = new AccountCreation(userInfo);
            Hashing hashing = new Hashing();
            Stopwatch timer = new Stopwatch();

            Credential removeMeLater = Credential.CreateSAUser();
            SealedSqlDAO SQLDao = new SealedSqlDAO(removeMeLater);
            var builder = new CustomSqlCommandBuilder();

            //last name is null so will throw an error 
            var validUserInfo = new UserInfo{
                username = "neDWdwtest@example.com",
                dob = new DateTime(1990, 1, 1),
                firstname = "Jane",
                lastname = "", 
                role = 5,
                status = "yes", 
                backupEmail = "test@backup.com"
            };

            SealedPepperDAO pepperDao = new SealedPepperDAO("C:/Users/kayka/Downloads/pepper.txt");
            string pepper = await pepperDao.ReadPepperAsync();

            var validPepper = new UserPepper{
                hashedUsername = hashing.HashData(validUserInfo.username, pepper)
            };
    
            var userAccount_success_parameters = new Dictionary<string, object>
            {
                { "username", validUserInfo.username},
                {"birthDate", validUserInfo.dob}   
            };

            var userProfile_success_parameters = new Dictionary<string, object>
            {
                {"hashedUsername", validPepper.hashedUsername},
                { "FirstName", validUserInfo.firstname},
                { "LastName", validUserInfo.lastname}, 
                {"backupEmail", validUserInfo.backupEmail},
                {"appRole", validUserInfo.role}, 
            };
            
            var activeAccount_success_parameters = new Dictionary<string, object>
            {
                {"hashedUsername", validPepper.hashedUsername},
                {"isActive", validUserInfo.status} 
            };

            var hashedAccount_success_parameters = new Dictionary<string, object>
                {
                    {"hashedUsername", validPepper.hashedUsername},
                    {"username", validUserInfo.username},
                };

            var tableData = new Dictionary<string, Dictionary<string, object>>
            {
                { "userAccount", userAccount_success_parameters },
                { "userProfile", userProfile_success_parameters },
                { "activeAccount", activeAccount_success_parameters}, 
                {"userHash", hashedAccount_success_parameters}
            };


            timer.Start();
            var response = await accountcreation.CreateUserAccount(validPepper, validUserInfo, tableData);
            timer.Stop();

            Assert.IsTrue(response.HasError, response.ErrorMessage);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 5000);
            await CleanupTestData().ConfigureAwait(false);
          
        }

        
        [TestMethod]
        public async Task CreateUserAccount_InvalidRole()
        {
            // AccountCreation accountcreation = new AccountCreation(SqlDAO sqlDao, ICustomSqlCommandBuilder commandBuilder);
            UserInfo userInfo = new UserInfo();
            UserPepper userPepper = new UserPepper();
            AccountCreation accountcreation = new AccountCreation(userInfo);
            Hashing hashing = new Hashing();
            Stopwatch timer = new Stopwatch();

            Credential removeMeLater = Credential.CreateSAUser();
            SealedSqlDAO SQLDao = new SealedSqlDAO(removeMeLater);
            var builder = new CustomSqlCommandBuilder();

            //role is invalid so will throw an error 
            var validUserInfo = new UserInfo{
                username = "idontkaddnonewtest@example.com",
                dob = new DateTime(1990, 1, 1),
                firstname = "Jane",
                lastname = "Doe", 
                role = 56,
                status = "yes", 
                backupEmail = "test@backup.com"
            };

            
            SealedPepperDAO pepperDao = new SealedPepperDAO("C:/Users/kayka/Downloads/pepper.txt");
            string pepper = await pepperDao.ReadPepperAsync();

            var validPepper = new UserPepper{
                hashedUsername = hashing.HashData(validUserInfo.username, pepper)
            };
    
            var userAccount_success_parameters = new Dictionary<string, object>
            {
                { "username", validUserInfo.username},
                {"birthDate", validUserInfo.dob}   
            };

            var userProfile_success_parameters = new Dictionary<string, object>
            {
                {"hashedUsername", validPepper.hashedUsername},
                { "FirstName", validUserInfo.firstname},
                { "LastName", validUserInfo.lastname}, 
                {"backupEmail", validUserInfo.backupEmail},
                {"appRole", validUserInfo.role}, 
            };
            
            var activeAccount_success_parameters = new Dictionary<string, object>
            {
                {"hashedUsername", validPepper.hashedUsername},
                {"isActive", validUserInfo.status} 
            };

            var hashedAccount_success_parameters = new Dictionary<string, object>
                {
                    {"hashedUsername", validPepper.hashedUsername},
                    {"username", validUserInfo.username},
                };

            var tableData = new Dictionary<string, Dictionary<string, object>>
            {
                { "userAccount", userAccount_success_parameters },
                { "userProfile", userProfile_success_parameters },
                { "activeAccount", activeAccount_success_parameters}, 
                {"userHash", hashedAccount_success_parameters}
            };


            timer.Start();
            var response = await accountcreation.CreateUserAccount(validPepper, validUserInfo, tableData);
            timer.Stop();
            Assert.IsTrue(response.HasError, response.ErrorMessage);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 5000);
            await CleanupTestData().ConfigureAwait(false);
          
        }



        [TestMethod]
        public async Task CreateUserAccount_InvaliStatus()
        {
            // AccountCreation accountcreation = new AccountCreation(SqlDAO sqlDao, ICustomSqlCommandBuilder commandBuilder);
            UserInfo userInfo = new UserInfo();
            UserPepper userPepper = new UserPepper();
            AccountCreation accountcreation = new AccountCreation(userInfo);
            Hashing hashing = new Hashing();
            Stopwatch timer = new Stopwatch();

            Credential removeMeLater = Credential.CreateSAUser();
            SealedSqlDAO SQLDao = new SealedSqlDAO(removeMeLater);
            var builder = new CustomSqlCommandBuilder();

        
            var validUserInfo = new UserInfo{
                username = "idontkaddnonewtest@example.com",
                dob = new DateTime(1990, 1, 1),
                firstname = "Jane",
                lastname = "Doe", 
                role = 2,
                status = "", 
                backupEmail = "test@backup.com"
            };

           
            SealedPepperDAO pepperDao = new SealedPepperDAO("C:/Users/kayka/Downloads/pepper.txt");
            string pepper = await pepperDao.ReadPepperAsync();

            var validPepper = new UserPepper{
                hashedUsername = hashing.HashData(validUserInfo.username, pepper)
            };
    
            var userAccount_success_parameters = new Dictionary<string, object>
            {
                { "username", validUserInfo.username},
                {"birthDate", validUserInfo.dob}   
            };

            var userProfile_success_parameters = new Dictionary<string, object>
            {
                {"hashedUsername", validPepper.hashedUsername},
                { "FirstName", validUserInfo.firstname},
                { "LastName", validUserInfo.lastname}, 
                {"backupEmail", validUserInfo.backupEmail},
                {"appRole", validUserInfo.role}, 
            };
            
            var activeAccount_success_parameters = new Dictionary<string, object>
            {
                {"hashedUsername", validPepper.hashedUsername},
                {"isActive", validUserInfo.status} 
            };

            var hashedAccount_success_parameters = new Dictionary<string, object>
                {
                    {"hashedUsername", validPepper.hashedUsername},
                    {"username", validUserInfo.username},
                };

            var tableData = new Dictionary<string, Dictionary<string, object>>
            {
                { "userAccount", userAccount_success_parameters },
                { "userProfile", userProfile_success_parameters },
                { "activeAccount", activeAccount_success_parameters}, 
                {"userHash", hashedAccount_success_parameters}
            };


            timer.Start();
            var response = await accountcreation.CreateUserAccount(validPepper, validUserInfo, tableData);
            timer.Stop();
            Assert.IsTrue(response.HasError, response.ErrorMessage);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 5000);
            await CleanupTestData().ConfigureAwait(false);
          
        }

        [TestMethod]
        public async Task CreateUserAccount_InvalidBackupEmail()
        {
            // AccountCreation accountcreation = new AccountCreation(SqlDAO sqlDao, ICustomSqlCommandBuilder commandBuilder);
            UserInfo userInfo = new UserInfo();
            UserPepper userPepper = new UserPepper();
            AccountCreation accountcreation = new AccountCreation(userInfo);
            Hashing hashing = new Hashing();
            Stopwatch timer = new Stopwatch();

            Credential removeMeLater = Credential.CreateSAUser();
            SealedSqlDAO SQLDao = new SealedSqlDAO(removeMeLater);
            var builder = new CustomSqlCommandBuilder();

  
            var validUserInfo = new UserInfo{
                username = "idontkaddnonewtest@example.com",
                dob = new DateTime(1990, 1, 1),
                firstname = "Jane",
                lastname = "Doe", 
                role = 3,
                status = "yes", 
                backupEmail = ""
            };

          
            SealedPepperDAO pepperDao = new SealedPepperDAO("C:/Users/kayka/Downloads/pepper.txt");
            string pepper = await pepperDao.ReadPepperAsync();

            var validPepper = new UserPepper{
                hashedUsername = hashing.HashData(validUserInfo.username, pepper)
            };
    
            var userAccount_success_parameters = new Dictionary<string, object>
            {
                { "username", validUserInfo.username},
                {"birthDate", validUserInfo.dob}   
            };

            var userProfile_success_parameters = new Dictionary<string, object>
            {
                {"hashedUsername", validPepper.hashedUsername},
                { "FirstName", validUserInfo.firstname},
                { "LastName", validUserInfo.lastname}, 
                {"backupEmail", validUserInfo.backupEmail},
                {"appRole", validUserInfo.role}, 
            };
            
            var activeAccount_success_parameters = new Dictionary<string, object>
            {
                {"hashedUsername", validPepper.hashedUsername},
                {"isActive", validUserInfo.status} 
            };

            var hashedAccount_success_parameters = new Dictionary<string, object>
                {
                    {"hashedUsername", validPepper.hashedUsername},
                    {"username", validUserInfo.username},
                };

            var tableData = new Dictionary<string, Dictionary<string, object>>
            {
                { "userAccount", userAccount_success_parameters },
                { "userProfile", userProfile_success_parameters },
                { "activeAccount", activeAccount_success_parameters}, 
                {"userHash", hashedAccount_success_parameters}
            };


            timer.Start();
            var response = await accountcreation.CreateUserAccount(validPepper, validUserInfo, tableData);
            timer.Stop();
            Assert.IsTrue(response.HasError, response.ErrorMessage);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 5000);
            await CleanupTestData().ConfigureAwait(false);
        }


        [TestMethod]
        public async Task CreateUserAccount_Timeout()
        {
            // AccountCreation accountcreation = new AccountCreation(SqlDAO sqlDao, ICustomSqlCommandBuilder commandBuilder);
            UserInfo userInfo = new UserInfo();
            UserPepper userPepper = new UserPepper();
            AccountCreation accountcreation = new AccountCreation(userInfo);
            Hashing hashing = new Hashing();
            Stopwatch timer = new Stopwatch();

            Credential removeMeLater = Credential.CreateSAUser();
            SealedSqlDAO SQLDao = new SealedSqlDAO(removeMeLater);
            var builder = new CustomSqlCommandBuilder();

        
            var validUserInfo = new UserInfo{
                username = "idontkaddnonewtest@example.com",
                dob = new DateTime(1990, 1, 1),
                firstname = "Jane",
                lastname = "Doe", 
                role = 1,
                status = "yes", 
                backupEmail = "test@backup.com"
            };

          
            SealedPepperDAO pepperDao = new SealedPepperDAO("C:/Users/kayka/Downloads/pepper.txt");
            string pepper = await pepperDao.ReadPepperAsync();

            var validPepper = new UserPepper{
                hashedUsername = hashing.HashData(validUserInfo.username, pepper)
            };
    
            var userAccount_success_parameters = new Dictionary<string, object>
            {
                { "username", validUserInfo.username},
                {"birthDate", validUserInfo.dob}   
            };

            var userProfile_success_parameters = new Dictionary<string, object>
            {
                {"hashedUsername", validPepper.hashedUsername},
                { "FirstName", validUserInfo.firstname},
                { "LastName", validUserInfo.lastname}, 
                {"backupEmail", validUserInfo.backupEmail},
                {"appRole", validUserInfo.role}, 
            };
            
            var activeAccount_success_parameters = new Dictionary<string, object>
            {
                {"hashedUsername", validPepper.hashedUsername},
                {"isActive", validUserInfo.status} 
            };

            var hashedAccount_success_parameters = new Dictionary<string, object>
                {
                    {"hashedUsername", validPepper.hashedUsername},
                    {"username", validUserInfo.username},
                };

            var tableData = new Dictionary<string, Dictionary<string, object>>
            {
                { "userAccount", userAccount_success_parameters },
                { "userProfile", userProfile_success_parameters },
                { "activeAccount", activeAccount_success_parameters}, 
                {"userHash", hashedAccount_success_parameters}
            };

            timer.Start();
            var response = await accountcreation.CreateUserAccount(validPepper, validUserInfo, tableData);
            timer.Stop();
            Assert.IsTrue(response.HasError, response.ErrorMessage);
            Assert.IsFalse(timer.ElapsedMilliseconds <= 1);
            await CleanupTestData().ConfigureAwait(false);
        }
        [TestMethod]
        public async Task CreateUserAccount_InvalidTableData()
        {
            // AccountCreation accountcreation = new AccountCreation(SqlDAO sqlDao, ICustomSqlCommandBuilder commandBuilder);
            UserInfo userInfo = new UserInfo();
            UserPepper userPepper = new UserPepper();
            AccountCreation accountcreation = new AccountCreation(userInfo);
            Hashing hashing = new Hashing();
            Stopwatch timer = new Stopwatch();

            Credential removeMeLater = Credential.CreateSAUser();
            SealedSqlDAO SQLDao = new SealedSqlDAO(removeMeLater);
            var builder = new CustomSqlCommandBuilder();

     
            var validUserInfo = new UserInfo{
                username = "unique233@example.com",
                dob = new DateTime(1990, 1, 1),
                firstname = "Jane",
                lastname = "Doe", 
                role = 5,
                status = "yes", 
                backupEmail = "test@backup.com"
            };

          
            SealedPepperDAO pepperDao = new SealedPepperDAO("C:/Users/kayka/Downloads/pepper.txt");
            string pepper = await pepperDao.ReadPepperAsync();

            var validPepper = new UserPepper{
                hashedUsername = hashing.HashData(validUserInfo.username, pepper)
            };
    
            var userAccount_success_parameters = new Dictionary<string, object>
            {
                { "THISISNOTACOLUMN", validUserInfo.username},
                {"birthDate", validUserInfo.dob}   
            };

            var userProfile_success_parameters = new Dictionary<string, object>
            {
                {"hashedUsername", validPepper.hashedUsername},
                { "FirstName", validUserInfo.firstname},
                { "LastName", validUserInfo.lastname}, 
                {"backupEmail", validUserInfo.backupEmail},
                {"appRole", validUserInfo.role}, 
            };
            
            var activeAccount_success_parameters = new Dictionary<string, object>
            {
                {"hashedUsername", validPepper.hashedUsername},
                {"isActive", validUserInfo.status} 
            };

            var hashedAccount_success_parameters = new Dictionary<string, object>
                {
                    {"hashedUsername", validPepper.hashedUsername},
                    {"username", validUserInfo.username},
                };

            var tableData = new Dictionary<string, Dictionary<string, object>>
            {
                { "userAccount", userAccount_success_parameters },
                { "userProfile", userProfile_success_parameters },
                { "activeAccount", activeAccount_success_parameters}, 
                {"userHash", hashedAccount_success_parameters}
            };

            timer.Start();
            var response = await accountcreation.CreateUserAccount(validPepper, validUserInfo, tableData);
            timer.Stop();
            Assert.IsTrue(response.HasError, response.ErrorMessage);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 5000);
            await CleanupTestData().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task CreateUserAccount_InvalidTable()
        {
            // AccountCreation accountcreation = new AccountCreation(SqlDAO sqlDao, ICustomSqlCommandBuilder commandBuilder);
            UserInfo userInfo = new UserInfo();
            UserPepper userPepper = new UserPepper();
            AccountCreation accountcreation = new AccountCreation(userInfo);
            Hashing hashing = new Hashing();
            Stopwatch timer = new Stopwatch();

            Credential removeMeLater = Credential.CreateSAUser();
            SealedSqlDAO SQLDao = new SealedSqlDAO(removeMeLater);
            var builder = new CustomSqlCommandBuilder();

          
            var validUserInfo = new UserInfo{
                username = "NOWWWPLEASWORKtabledoesnotexits@example.com",
                dob = new DateTime(1990, 1, 1),
                firstname = "Jane",
                lastname = "Doe", 
                role = 4,
                status = "yes", 
                backupEmail = "test@backup.com"
            };

        
            SealedPepperDAO pepperDao = new SealedPepperDAO("C:/Users/kayka/Downloads/pepper.txt");
            string pepper = await pepperDao.ReadPepperAsync();

            var validPepper = new UserPepper{
                hashedUsername = hashing.HashData(validUserInfo.username, pepper)
            };
    
            var userAccount_success_parameters = new Dictionary<string, object>
            {
                { "username", validUserInfo.username},
                {"birthDate", validUserInfo.dob}   
            };

            var userProfile_success_parameters = new Dictionary<string, object>
            {
                {"hashedUsername", validPepper.hashedUsername},
                { "FirstName", validUserInfo.firstname},
                { "LastName", validUserInfo.lastname}, 
                {"backupEmail", validUserInfo.backupEmail},
                {"appRole", validUserInfo.role}, 
            };
            
            var activeAccount_success_parameters = new Dictionary<string, object>
            {
                {"hashedUsername", validPepper.hashedUsername},
                {"isActive", validUserInfo.status} 
            };

            var hashedAccount_success_parameters = new Dictionary<string, object>
                {
                    {"hashedUsername", validPepper.hashedUsername},
                    {"username", validUserInfo.username},
                };

            var tableData = new Dictionary<string, Dictionary<string, object>>
            {
                { "THISISNOTATABLEWEHAVE", userAccount_success_parameters },
                { "userProfile", userProfile_success_parameters },
                { "activeAccount", activeAccount_success_parameters}, 
                {"userHash", hashedAccount_success_parameters}
            };

            timer.Start();
            var response = await accountcreation.CreateUserAccount(validPepper, validUserInfo, tableData);
            timer.Stop();
            Assert.IsTrue(response.HasError, response.ErrorMessage);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 5000);
            await CleanupTestData().ConfigureAwait(false);
        }


    }
}