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

        // [TestMethod]
        // public async Task ReadPepperFileContents_Success()
        // {
        //     string filePath = "C:/Users/kayka/Downloads/pepper.txt";
        //     SealedPepperDAO pepperDao = new SealedPepperDAO(filePath);
        //     string pepper = await pepperDao.ReadPepperAsync();
        //     Console.WriteLine($"Pepper content: {pepper}");
        //     Assert.IsFalse(string.IsNullOrEmpty(pepper), "The pepper file should not be empty.");
        // }


        // [TestMethod]
        // public async Task isTHISVALID()
        // {
        //     // AccountCreation accountcreation = new AccountCreation(SqlDAO sqlDao, ICustomSqlCommandBuilder commandBuilder);
        //     UserInfo userInfo = new UserInfo();
        //     UserPepper userPepper = new UserPepper();
        //     AccountCreation accountcreation = new AccountCreation(userInfo);
        //     Hashing hashing = new Hashing();
        //     Stopwatch timer = new Stopwatch();

        //     string configFilePath = "C:/Users/kayka/Downloads/config.local.txt";
        //     ConfigService configService = new ConfigService(configFilePath);
        //     var builder = new CustomSqlCommandBuilder();

        //     SealedPepperDAO pepperDao = new SealedPepperDAO("C:/Users/kayka/Downloads/pepper.txt");
        //     string pepper = await pepperDao.ReadPepperAsync();

        //     //username must be unique in database
        //     var validUserInfo = new UserInfo
        //     {
        //         username = "memem@hotmail.com",
        //         dob = new DateTime(1990, 1, 1),
        //         firstname = "vgggggggggggggggggggggggggggggkjkjjjjhjjjjjjjjjjjjhjfffgf",
        //         lastname = "k", 
        //         role = 1,
        //         status = "yes", 
        //         backupEmail = "test@backup.com"
        //     };


        //     timer.Start();
        //     string didPASS = accountcreation.CheckUserInfoValidity(validUserInfo);
        //     timer.Stop();

        //     // Assert.IsFalse(response.HasError, response.ErrorMessage);
        //     // Assert.IsTrue(timer.ElapsedMilliseconds <= 5000);
        //     await CleanupTestData().ConfigureAwait(false);
          
        // }



        [TestMethod]
        public async Task CreateUserAccount_Success()
        {
            // AccountCreation accountcreation = new AccountCreation(SqlDAO sqlDao, ICustomSqlCommandBuilder commandBuilder);
            UserInfo userInfo = new UserInfo();
            AccountCreation accountCreation = new AccountCreation();
            Stopwatch timer = new Stopwatch();

            //username must be unique in database
            var validUserInfo = new UserInfo
            {
                username = "tryingcompanyeamil@hotmail.com",
                dob = new DateTime(1990, 1, 1),
                firstname = "CONFIG",
                lastname = "CONDFIG", 
                role = 1,
                status = "yes", 
                backupEmail = "COMBININGEVERYTHING@backup.com"
            };

            timer.Start();
            var response = await accountCreation.CreateUserAccount(validUserInfo);
            timer.Stop();

            Assert.IsFalse(response.HasError, response.ErrorMessage);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 5000);
            await CleanupTestData().ConfigureAwait(false);
          
        }


        [TestMethod]
        public async Task CreateManagerAccount_Success()
        {
            // AccountCreation accountcreation = new AccountCreation(SqlDAO sqlDao, ICustomSqlCommandBuilder commandBuilder);
            AccountCreation accountcreation = new AccountCreation();
            Stopwatch timer = new Stopwatch();
            //username must be unique in database
            var validUserInfo = new UserInfo
            {
                username = "newbsuinessemail@hotmail.com",
                dob = new DateTime(1990, 1, 1),
                firstname = "NewCompanyBusinessCofig",
                lastname = "NewCompanyBusinessCofig", 
                role = 2,
                status = "yes", 
                backupEmail = "test@backup.com", 
                companyName = "NewCompanyBusinessCofig", 
                address = "Irvine", 
                openingHours = "2:00:00",
                closingHours = "2:00:00" ,
                daysOpen = "Monday,Tuesday"
            };

            
            timer.Start();
            var response = await accountcreation.CreateUserAccount(validUserInfo);
            timer.Stop();

            Assert.IsFalse(response.HasError, response.ErrorMessage);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 5000);
            await CleanupTestData().ConfigureAwait(false);
          
        }


        [TestMethod]
        public async Task CreateUserAccount_InvalidUsername()
        {
            // AccountCreation accountcreation = new AccountCreation(SqlDAO sqlDao, ICustomSqlCommandBuilder commandBuilder);
            AccountCreation accountcreation = new AccountCreation();
            Hashing hashing = new Hashing();
            Stopwatch timer = new Stopwatch();

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
            timer.Start();
            var response = await accountcreation.CreateUserAccount(validUserInfo);
            timer.Stop();

            Assert.IsTrue(response.HasError, response.ErrorMessage);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 5000);
            await CleanupTestData().ConfigureAwait(false);
        }


        [TestMethod]
        public async Task CreateUserAccount_InvalidFirstName()
        {
            // AccountCreation accountcreation = new AccountCreation(SqlDAO sqlDao, ICustomSqlCommandBuilder commandBuilder);
            AccountCreation accountcreation = new AccountCreation();
            Hashing hashing = new Hashing();
            Stopwatch timer = new Stopwatch();

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
            timer.Start();
            var response = await accountcreation.CreateUserAccount(validUserInfo);
            timer.Stop();

            Assert.IsTrue(response.HasError, response.ErrorMessage);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 5000);
            await CleanupTestData().ConfigureAwait(false);
          
        }


        [TestMethod]
        public async Task CreateUserAccount_InvalidLastName()
        {
            // AccountCreation accountcreation = new AccountCreation(SqlDAO sqlDao, ICustomSqlCommandBuilder commandBuilder);
            AccountCreation accountcreation = new AccountCreation();
            Hashing hashing = new Hashing();
            Stopwatch timer = new Stopwatch();

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
            timer.Start();
            var response = await accountcreation.CreateUserAccount(validUserInfo);
            timer.Stop();

            Assert.IsTrue(response.HasError, response.ErrorMessage);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 5000);
            await CleanupTestData().ConfigureAwait(false);
          
        }

        
        [TestMethod]
        public async Task CreateUserAccount_InvalidRole()
        {
            // AccountCreation accountcreation = new AccountCreation(SqlDAO sqlDao, ICustomSqlCommandBuilder commandBuilder);
            AccountCreation accountcreation = new AccountCreation();
            Hashing hashing = new Hashing();
            Stopwatch timer = new Stopwatch();

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
            timer.Start();
            var response = await accountcreation.CreateUserAccount(validUserInfo);
            timer.Stop();
            Assert.IsTrue(response.HasError, response.ErrorMessage);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 5000);
            await CleanupTestData().ConfigureAwait(false);
          
        }



        [TestMethod]
        public async Task CreateUserAccount_InvaliStatus()
        {
            // AccountCreation accountcreation = new AccountCreation(SqlDAO sqlDao, ICustomSqlCommandBuilder commandBuilder);
            AccountCreation accountcreation = new AccountCreation();
            Hashing hashing = new Hashing();
            Stopwatch timer = new Stopwatch();
            var validUserInfo = new UserInfo{
                username = "idontkaddnonewtest@example.com",
                dob = new DateTime(1990, 1, 1),
                firstname = "Jane",
                lastname = "Doe", 
                role = 1,
                status = "", 
                backupEmail = "test@backup.com"
            };

            timer.Start();
            var response = await accountcreation.CreateUserAccount(validUserInfo);
            timer.Stop();
            Assert.IsTrue(response.HasError, response.ErrorMessage);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 5000);
            await CleanupTestData().ConfigureAwait(false);
          
        }

        [TestMethod]
        public async Task CreateUserAccount_InvalidBackupEmail()
        {
            // AccountCreation accountcreation = new AccountCreation(SqlDAO sqlDao, ICustomSqlCommandBuilder commandBuilder);
            AccountCreation accountcreation = new AccountCreation();
            Hashing hashing = new Hashing();
            Stopwatch timer = new Stopwatch();
            var validUserInfo = new UserInfo{
                username = "idontkaddnonewtest@example.com",
                dob = new DateTime(1990, 1, 1),
                firstname = "Jane",
                lastname = "Doe", 
                role = 4,
                status = "yes", 
                backupEmail = ""
            };

            timer.Start();
            var response = await accountcreation.CreateUserAccount(validUserInfo);
            timer.Stop();
            Assert.IsTrue(response.HasError, response.ErrorMessage);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 5000);
            await CleanupTestData().ConfigureAwait(false);
        }


        [TestMethod]
        public async Task CreateUserAccount_Timeout()
        {
            // AccountCreation accountcreation = new AccountCreation(SqlDAO sqlDao, ICustomSqlCommandBuilder commandBuilder);

            AccountCreation accountcreation = new AccountCreation();
            Hashing hashing = new Hashing();
            Stopwatch timer = new Stopwatch();

            var validUserInfo = new UserInfo{
                username = "idontkaddnonewtest@example.com",
                dob = new DateTime(1990, 1, 1),
                firstname = "Jane",
                lastname = "Doe", 
                role = 1,
                status = "yes", 
                backupEmail = "test@backup.com"
            };

            timer.Start();
            var response = await accountcreation.CreateUserAccount(validUserInfo);
            timer.Stop();
            Assert.IsTrue(response.HasError, response.ErrorMessage);
            Assert.IsFalse(timer.ElapsedMilliseconds <= 1);
            await CleanupTestData().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task CreateUserAccount_InvalidTableData()
        {
            // AccountCreation accountcreation = new AccountCreation(SqlDAO sqlDao, ICustomSqlCommandBuilder commandBuilder);
            AccountCreation accountcreation = new AccountCreation();
            Stopwatch timer = new Stopwatch();

            var validUserInfo = new UserInfo{
                username = "unique233@example.com",
                dob = new DateTime(1990, 1, 1),
                firstname = "Jane",
                lastname = "Doe", 
                role = 5,
                status = "yes", 
                backupEmail = "test@backup.com"
            };

            timer.Start();
            var response = await accountcreation.CreateUserAccount(validUserInfo);
            timer.Stop();
            Assert.IsTrue(response.HasError, response.ErrorMessage);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 5000);
            await CleanupTestData().ConfigureAwait(false);
        }

    

    }
}