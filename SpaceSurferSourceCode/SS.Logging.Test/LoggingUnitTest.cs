using SS.Logging.DataAccess;
using SS.SharedNamespace;
using System.Data.SqlClient;
using System.Diagnostics;
using SS.Logging.LogTarget;


namespace SS.Logging.Test
{
    [TestClass]
    public class LoggingUnitTest
    {
        private SqlDAO? dao;

        [TestInitialize]
        public void TestInitialize()
        {
            var SAUser = Credential.CreateSAUser();
            dao = new SqlDAO(SAUser);
        }

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
        public async Task SaveData_LogLevel_Info_Pass()
        {
            // Arrange
            Logger logger = new Logger(dao);
            var log = new LogEntry
            {
                level = "Info",
                username = "test@email",
                category = "View",
                description = "Testing logging info level..."
            };
            Stopwatch timer = new Stopwatch();

            // Act
            timer.Start();
            var result = await logger.SaveData(log).ConfigureAwait(false);
            timer.Stop();

            // Assert
            Assert.IsFalse(result.HasError);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

            //Cleanup
            await CleanupTestData().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SaveData_LogLevel_Debug_Pass()
        {
            // Arrange
            Logger logger = new Logger(dao);
            var log = new LogEntry
            {
                level = "Debug",
                username = "test@email",
                category = "View",
                description = "Testing logging debug level..."
            };
            Stopwatch timer = new Stopwatch();

            // Act
            timer.Start();
            var result = await logger.SaveData(log).ConfigureAwait(false);
            timer.Stop();

            // Assert
            Assert.IsFalse(result.HasError);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

            //Cleanup
            await CleanupTestData().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SaveData_LogLevel_Warning_Pass()
        {
            // Arrange
            Logger logger = new Logger(dao);
            var log = new LogEntry
            {
                level = "Warning",
                username = "test@email",
                category = "View",
                description = "Testing logging warning level..."
            };
            Stopwatch timer = new Stopwatch();

            // Act
            timer.Start();
            var result = await logger.SaveData(log).ConfigureAwait(false);
            timer.Stop();

            // Assert
            Assert.IsFalse(result.HasError);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

            //Cleanup
            await CleanupTestData().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SaveData_LogLevel_Error_Pass()
        {
            // Arrange
            Logger logger = new Logger(dao);
            var log = new LogEntry
            {
                level = "Error",
                username = "test@email",
                category = "View",
                description = "Testing logging error level..."
            };
            Stopwatch timer = new Stopwatch();

            // Act
            timer.Start();
            var result = await logger.SaveData(log).ConfigureAwait(false);
            timer.Stop();

            // Assert
            Assert.IsFalse(result.HasError);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

            //Cleanup
            await CleanupTestData().ConfigureAwait(false);
        }
        [TestMethod]
        public async Task SaveData_LogLevel_InvalidLevel_Pass()
        {
            // Arrange
            Logger logger = new Logger(dao);
            var log = new LogEntry
            {
                level = "Invalid",
                username = "test@email",
                category = "View",
                description = "Testing logging an invalid level..."
            };

            // Act
            var result = await logger.SaveData(log).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result.HasError);

            //Cleanup
            await CleanupTestData().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SaveData_Category_View_Pass()
        {
            // Arrange
            Logger logger = new Logger(dao);
            var log = new LogEntry
            {
                level = "Debug",
                username = "test@email",
                category = "View",
                description = "Testing logging view category..."
            };
            Stopwatch timer = new Stopwatch();

            // Act
            timer.Start();
            var result = await logger.SaveData(log).ConfigureAwait(false);
            timer.Stop();

            // Assert
            Assert.IsFalse(result.HasError);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

            //Cleanup
            await CleanupTestData().ConfigureAwait(false);
        }
        [TestMethod]
        public async Task SaveData_Category_Business_Pass()
        {
            // Arrange
            Logger logger = new Logger(dao);
            var log = new LogEntry
            {
                level = "Debug",
                username = "test@email",
                category = "Business",
                description = "Testing business category..."
            };
            Stopwatch timer = new Stopwatch();

            // Act
            timer.Start();
            var result = await logger.SaveData(log).ConfigureAwait(false);
            timer.Stop();

            // Assert
            Assert.IsFalse(result.HasError);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

            //Cleanup
            await CleanupTestData().ConfigureAwait(false);
        }
        [TestMethod]
        public async Task SaveData_Category_Server_Pass()
        {
            // Arrange
            Logger logger = new Logger(dao);
            var log = new LogEntry
            {
                level = "Debug",
                username = "test@email",
                category = "Server",
                description = "Testing logging server category..."
            };
            Stopwatch timer = new Stopwatch();

            // Act
            timer.Start();
            var result = await logger.SaveData(log).ConfigureAwait(false);
            timer.Stop();

            // Assert
            Assert.IsFalse(result.HasError);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

            //Cleanup
            await CleanupTestData().ConfigureAwait(false);
        }
        public async Task SaveData_Category_Data_Pass()
        {
            // Arrange
            Logger logger = new Logger(dao);
            var log = new LogEntry
            {
                level = "Debug",
                username = "test@email",
                category = "Data",
                description = "Testing logging data category..."
            };
            Stopwatch timer = new Stopwatch();

            // Act
            timer.Start();
            var result = await logger.SaveData(log).ConfigureAwait(false);
            timer.Stop();

            // Assert
            Assert.IsFalse(result.HasError);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

            //Cleanup
            await CleanupTestData().ConfigureAwait(false);
        }
        [TestMethod]
        public async Task SaveData_Category_DataStore_Pass()
        {
            // Arrange
            Logger logger = new Logger(dao);
            var log = new LogEntry
            {
                level = "Debug",
                username = "test@email",
                category = "Data Store",
                description = "Testing logging data store category..."
            };
            Stopwatch timer = new Stopwatch();

            // Act
            timer.Start();
            var result = await logger.SaveData(log).ConfigureAwait(false);
            timer.Stop();

            // Assert
            Assert.IsFalse(result.HasError);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

            //Cleanup
            await CleanupTestData().ConfigureAwait(false);
        }
        [TestMethod]
        public async Task SaveData_Category_InvalidCategory_Pass()
        {
            // Arrange
            Logger logger = new Logger(dao);
            var log = new LogEntry
            {
                level = "Debug",
                username = "test@email",
                category = "Invalid",
                description = "Testing logging invalid category..."
            };

            // Act
            var result = await logger.SaveData(log).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result.HasError);

            //Cleanup
            await CleanupTestData().ConfigureAwait(false);
        }
        [TestMethod]
        public async Task SaveData_Description_ValidLength_Pass()
        {
            // Arrange
            Logger logger = new Logger(dao);
            var log = new LogEntry
            {
                level = "Debug",
                username = "test@email",
                category = "View",
                description = "Testing description with valid length..."
            };
            Stopwatch timer = new Stopwatch();

            // Act
            timer.Start();
            var result = await logger.SaveData(log).ConfigureAwait(false);
            timer.Stop();

            // Assert
            Assert.IsFalse(result.HasError);
            Assert.IsTrue(timer.ElapsedMilliseconds <= 3000);

            //Cleanup
            await CleanupTestData().ConfigureAwait(false);
        }
        [TestMethod]
        public async Task SaveData_Description_InvalidLength_Pass()
        {
            // Arrange
            Logger logger = new Logger(dao);
            var log = new LogEntry
            {
                level = "Debug",
                username = "test@email",
                category = "View",
                description = "Amidst the vibrant tapestry of life, where the relentless " +
                "march of time intertwines with the intricate threads of human experience, " +
                "a kaleidoscope of emotions, aspirations, and challenges unfolds, weaving a " +
                "narrative that spans the spectrum of joy, sorrow, triumph, and introspection, " +
                "resonating through the corridors of existence as individuals navigate the " +
                "labyrinth of their own stories, seeking meaning and connection in the complex " +
                "mosaic of relationships, dreams, and the ever-evolving tapestry of the cosmos, " +
                "where each moment is a brushstroke on the canvas of eternity, a fleeting expression " +
                "in the grand symphony of life that echoes across the vast expanse of the universe, " +
                "harmonizing with the cosmic dance of celestial bodies and the whispers of the unseen " +
                "forces that shape the destiny of worlds, creating an intricate ballet that transcends " +
                "the boundaries of time and space, inviting contemplation on the ephemeral nature of our " +
                "earthly sojourn and the profound interconnectedness that binds every sentient being to " +
                "the cosmic heartbeat, pulsating with the rhythm of creation, echoing the timeless refrain " +
                "of existence itself. \r\nIn the vast realm of technology, innovations continually shape our " +
                "daily lives. From the ubiquity of smartphones connecting us across continents to the intricate " +
                "algorithms powering search engines, technology permeates every facet of society. The " +
                "evolution of artificial intelligence promises both unprecedented opportunities and ethical " +
                "dilemmas. As automation becomes more prevalent, questions arise about the future of work and " +
                "the potential societal impacts. Meanwhile, the intricate dance between privacy and convenience " +
                "unfolds as data-driven technologies thrive. Cybersecurity stands as a sentinel, guarding against " +
                "the ever-present threat of digital incursions. Amid these advancements, the need for digital " +
                "literacy and ethical considerations becomes paramount. Striking a balance between progress " +
                "and responsibility is the defining challenge of the digital age. In this era of constant " +
                "connectivity, fostering a thoughtful relationship with technology is essential for navigating the " +
                "complex landscape it presents."
            };

            // Act
            var result = await logger.SaveData(log).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result.HasError);

            //Cleanup
            await CleanupTestData().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Text_File_Log_Target_Success()
        {
            // Arrange

            var textFileLogger = new TextFileLogTarget("/Users/carinegordillo/Downloads/SS-4.Logging/File_Logger_Test.txt");
            Logger logger = new Logger(textFileLogger);
            var log = new LogEntry
            {
                level = "Debug",
                username = "SpaceSurfer@email",
                category = "View",
                description = "Testing File Logger"
            };
            Stopwatch timer = new Stopwatch();


            // Act
            timer.Start();
            var result = await logger.SaveData(log).ConfigureAwait(false);
            timer.Stop();

            // Assert
            Assert.IsFalse(result.HasError);
            //Cleanup
            await CleanupTestData().ConfigureAwait(false);
        }

        [TestMethod]
        //is this a repetative test ?
        public async Task SQLDAO_Log_Target_Success()
        {
            // Arrange
            Logger logger = new Logger(dao);
            var log = new LogEntry
            {
                level = "Debug",
                username = "test@email",
                category = "View",
                description = "Testing description with valid length..."
            };
            Stopwatch timer = new Stopwatch();
            

            // Act
            timer.Start();
            var result = await logger.SaveData(log).ConfigureAwait(false);
            timer.Stop();
            // Assert
            Assert.IsFalse(result.HasError);
            //Cleanup
            await CleanupTestData().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Console_Log_Target_Success()
        {
            // Arrange
            var consolelogger = new ConsoleLogTarget();
            Logger logger = new Logger(consolelogger);
            var log = new LogEntry
            {
                level = "Debug",
                username = "SpaceSurfer@email",
                category = "View",
                description = "Testing"
            };
            Stopwatch timer = new Stopwatch();

            // Act
            timer.Start();
            var result = await logger.SaveData(log).ConfigureAwait(false);
            timer.Stop();
            // Assert
            Assert.IsFalse(result.HasError);
            //Cleanup
            await CleanupTestData().ConfigureAwait(false);
        }
        [TestMethod]
        public async Task CSV_File_Log_Target_Success()
        {
            // Arrange

            var CSVtextFileLogger = new CSVFileLogTarget("/Users/carinegordillo/Downloads/SS-4.Logging/CSV_File_LogTarget_Test.csv");
            Logger logger = new Logger(CSVtextFileLogger);
            var log = new LogEntry
            {
                level = "Error",
                username = "PixelPals@email",
                category = "Business",
                description = "Testing CSV Logger"
            };
            Stopwatch timer = new Stopwatch();

            // Act
            timer.Start();
            var result = await logger.SaveData(log).ConfigureAwait(false);
            timer.Stop();

            // Assert
            Assert.IsFalse(result.HasError);
            //Cleanup
            await CleanupTestData().ConfigureAwait(false);
        }
    }

}

