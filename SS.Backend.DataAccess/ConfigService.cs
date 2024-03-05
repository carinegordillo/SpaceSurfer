using System.Text;

namespace SS.Backend.DataAccess
{
    public class ConfigService
    {
        private readonly string configValues;

        public ConfigService(string configFilePath)
        {
            configValues = LoadConfig(configFilePath);
        }

        /// <summary>
        /// This method gets the connection string
        /// </summary>
        /// <returns>The connection string</returns>
        public string GetConnectionString()
        {
            return configValues;

            throw new InvalidOperationException("ConnectionString not found in the configuration file.");
        }

        /// <summary>
        /// This method loads in the config file and reads it.
        /// </summary>
        /// <param name="filePath">The path of where the config file is on your system</param>
        /// <returns>Each line read in</returns>
        private string LoadConfig(string filePath)
        {
            string config = "";
            try
            {
                var lines = File.ReadAllLines(filePath, Encoding.UTF8);
                if (lines.Length > 0)
                {
                    config = lines[0];
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading configuration file: {ex.Message}");
                throw new InvalidOperationException($"Error loading configuration file: {ex.Message}", ex);
            }

            return config;

        }


    }
}

