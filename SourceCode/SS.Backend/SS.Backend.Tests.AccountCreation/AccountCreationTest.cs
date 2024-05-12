using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Text.Json;
using SS.Backend.UserManagement;
using System.Diagnostics;
using SS.Backend.Services.LoggingService;
namespace SS.Backend.Tests.AccountCreationTest
{
    [TestClass]
    public class AccountCreationTests
    {
        private ConfigService _configService;
        private AccountCreation  _accountCreation;

        private UserManagementDao _userManagementDao;
        private SqlDAO _sqlDao;
        private ILogger _logger;


        [TestInitialize]
        public void Setup()
        {
            var baseDirectory = AppContext.BaseDirectory;
            var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
            var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
            _configService = new ConfigService(configFilePath);
            _sqlDao = new SqlDAO(_configService);

            _userManagementDao = new UserManagementDao(_sqlDao);
            _accountCreation = new AccountCreation(_userManagementDao, _logger);
        }

        [TestCleanup]
        public async Task TestCleanup()
        {
            await CleanupTestData();
        }

        private async Task CleanupTestData()
        {
           
            try
            {
                string sqlCMD = "DELETE FROM dbo.taskHub WHERE hashedUsername = 'bdPoc6J/PLVi3kxY4iaQejK+hp+dNUeEd4CMpfBPQSE='";
                var cmd = new SqlCommand(sqlCMD);
                var response = await _sqlDao.SqlRowsAffected(cmd);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during test cleanup: {ex}");
            }
        }
        
        [TestMethod]
        public async Task CreateUserAccount_Success()
        {
            // AccountCreation accountcreation = new AccountCreation(SqlDAO sqlDao, ICustomSqlCommandBuilder commandBuilder);
            UserInfo userInfo = new UserInfo();
            //AccountCreation accountCreation = new AccountCreation();
            Stopwatch timer = new Stopwatch();

            //username must be unique in database
            var validUserInfo = new UserInfo
            {
                username = "unittestemail@hotmail.com",
                dob = new DateTime(1990, 1, 1),
                firstname = "test",
                lastname = "email", 
                role = 5,
                status = "no", 
                backupEmail = "COMBININGEVERYTHING@backup.com"
            };

            timer.Start();
            var response = await _accountCreation.CreateUserAccount(validUserInfo, null, null);
            timer.Stop();

            Assert.IsFalse(response.HasError, response.ErrorMessage);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 5000);
            //await CleanupTestData().ConfigureAwait(false);
          
        }

        [TestMethod]
        public async Task CreateEmployeeAccount_Success()
        {
            // AccountCreation accountcreation = new AccountCreation(SqlDAO sqlDao, ICustomSqlCommandBuilder commandBuilder);
            UserInfo userInfo = new UserInfo();
            //AccountCreation accountCreation = new AccountCreation();
            Stopwatch timer = new Stopwatch();

            //username must be unique in database
            var validUserInfo = new UserInfo
            {
                username = "employeework@hotmail.com",
                dob = new DateTime(1990, 1, 1),
                firstname = "workemployee",
                lastname = "workemployee", 
                role = 4,
                status = "no", 
                backupEmail = "COMBININGEVERYTHING@backup.com"
            };
            var manager_hashedUsername = "kj3VOKOk9Dh0pY5Fh41Dr7knV3/qR9FI6I7FmZlRVtc=";

            timer.Start();
            var response = await _accountCreation.CreateUserAccount(validUserInfo, null, manager_hashedUsername);
            timer.Stop();

            Assert.IsFalse(response.HasError, response.ErrorMessage);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 5000);
            //await CleanupTestData().ConfigureAwait(false);
          
        }
        
        


        [TestMethod]
        public async Task VerifyAccount_Success()
        {
            //AccountCreation accountCreation = new AccountCreation();
            Stopwatch timer = new Stopwatch();

            timer.Start();
            var response = await _accountCreation.VerifyAccount("kay.kayale@student.csulb.edu");
            timer.Stop();

            Assert.IsFalse(response.HasError, response.ErrorMessage);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 5000);
            //await CleanupTestData().ConfigureAwait(false);
          
        }


        [TestMethod]
        public async Task CreateManagerAccount_Success()
        {
            // AccountCreation accountcreation = new AccountCreation(SqlDAO sqlDao, ICustomSqlCommandBuilder commandBuilder);
            //AccountCreation accountCreation = new AccountCreation();
            Stopwatch timer = new Stopwatch();
            //username must be unique in database
            var validUserInfo = new UserInfo
            {
                username = "compnayfortestinf@hotmail.com",
                dob = new DateTime(1990, 1, 1),
                firstname = "TESTINFT",
                lastname = "TESTING", 
                role = 3,
                status = "yes", 
                backupEmail = "test@backup.com", 
            };

            var validCompanyInfo = new CompanyInfo
            {
                companyName = "Testing for Company", 
                address = "Irvine", 
                openingHours = "2:00:00",
                closingHours = "2:00:00" ,
                daysOpen = "Monday,Tuesday"
            };

            
            timer.Start();
            var response = await _accountCreation.CreateUserAccount(validUserInfo, validCompanyInfo, null);
            timer.Stop();

            Assert.IsFalse(response.HasError, response.ErrorMessage);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 5000);
            //await CleanupTestData().ConfigureAwait(false);
          
        }


        [TestMethod]
        public async Task CreateUserAccount_InvalidUsername()
        {
            // AccountCreation accountcreation = new AccountCreation(SqlDAO sqlDao, ICustomSqlCommandBuilder commandBuilder);
            //AccountCreation accountCreation = new AccountCreation();
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
            var response = await _accountCreation.CreateUserAccount(validUserInfo, null, null);
            timer.Stop();

            Assert.IsTrue(response.HasError, response.ErrorMessage);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 5000);
            //await CleanupTestData().ConfigureAwait(false);
        }


        [TestMethod]
        public async Task CreateUserAccount_InvalidFirstName()
        {
            // AccountCreation accountcreation = new AccountCreation(SqlDAO sqlDao, ICustomSqlCommandBuilder commandBuilder);
            //AccountCreation accountCreation = new AccountCreation();
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
            var response = await _accountCreation.CreateUserAccount(validUserInfo, null, null);
            timer.Stop();

            Assert.IsTrue(response.HasError, response.ErrorMessage);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 5000);
            //await CleanupTestData().ConfigureAwait(false);
          
        }


        [TestMethod]
        public async Task CreateUserAccount_InvalidLastName()
        {
            // AccountCreation accountcreation = new AccountCreation(SqlDAO sqlDao, ICustomSqlCommandBuilder commandBuilder);
            //AccountCreation accountCreation = new AccountCreation();
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
            var response = await _accountCreation.CreateUserAccount(validUserInfo, null, null);
            timer.Stop();

            Assert.IsTrue(response.HasError, response.ErrorMessage);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 5000);
            //await CleanupTestData().ConfigureAwait(false);
          
        }

        
        [TestMethod]
        public async Task CreateUserAccount_InvalidRole()
        {
            // AccountCreation accountcreation = new AccountCreation(SqlDAO sqlDao, ICustomSqlCommandBuilder commandBuilder);
            //AccountCreation accountCreation = new AccountCreation();
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
            var response = await _accountCreation.CreateUserAccount(validUserInfo, null, null);
            timer.Stop();
            Assert.IsTrue(response.HasError, response.ErrorMessage);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 5000);
            //await CleanupTestData().ConfigureAwait(false);
          
        }



        [TestMethod]
        public async Task CreateUserAccount_InvaliStatus()
        {
            // AccountCreation accountcreation = new AccountCreation(SqlDAO sqlDao, ICustomSqlCommandBuilder commandBuilder);
            //AccountCreation accountCreation = new AccountCreation();
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
            var response = await _accountCreation.CreateUserAccount(validUserInfo, null, null);
            timer.Stop();
            Assert.IsTrue(response.HasError, response.ErrorMessage);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 5000);
            //await CleanupTestData().ConfigureAwait(false);
          
        }

        [TestMethod]
        public async Task CreateUserAccount_InvalidBackupEmail()
        {
            // AccountCreation accountcreation = new AccountCreation(SqlDAO sqlDao, ICustomSqlCommandBuilder commandBuilder);
            //AccountCreation accountCreation = new AccountCreation();
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
            var response = await _accountCreation.CreateUserAccount(validUserInfo, null, null);
            timer.Stop();
            Assert.IsTrue(response.HasError, response.ErrorMessage);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 5000);
            //await CleanupTestData().ConfigureAwait(false);
        }


        [TestMethod]
        public async Task CreateUserAccount_Timeout()
        {
            // AccountCreation accountcreation = new AccountCreation(SqlDAO sqlDao, ICustomSqlCommandBuilder commandBuilder);

            //AccountCreation accountCreation = new AccountCreation();
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
            var response = await _accountCreation.CreateUserAccount(validUserInfo, null, null);
            timer.Stop();
            Assert.IsTrue(response.HasError, response.ErrorMessage);
            Assert.IsFalse(timer.ElapsedMilliseconds <= 1);
            //await CleanupTestData().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task CreateUserAccount_InvalidTableData()
        {
            // AccountCreation accountcreation = new AccountCreation(SqlDAO sqlDao, ICustomSqlCommandBuilder commandBuilder);
            //AccountCreation accountCreation = new AccountCreation();
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
            var response = await _accountCreation.CreateUserAccount(validUserInfo, null, null);
            timer.Stop();
            Assert.IsTrue(response.HasError, response.ErrorMessage);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 5000);
            //await CleanupTestData().ConfigureAwait(false);
        }

    

    }
}