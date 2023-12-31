﻿using System.Text;

namespace SS.Backend.DataAccess
{
    public class ConfigService
    {
        private readonly Dictionary<string, string> configValues;

        public ConfigService(string configFilePath)
        {
            configValues = LoadConfig(configFilePath);

            // Print the loaded configuration values to the console for debugging
            Console.WriteLine("Loaded Configuration Values:");
            foreach (var kvp in configValues)
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value}");
            }
        }

        /// <summary>
        /// This method gets the connection string
        /// </summary>
        /// <returns>The connection string</returns>
        public string GetConnectionString()
        {
            Console.WriteLine("Loaded Configuration Values:");
            foreach (var kvp in configValues)
            {
                Console.WriteLine($"{kvp.Key} = {kvp.Value}");
            }

            if (configValues.TryGetValue("ConnectionString", out var connectionString))
            {
                return connectionString;
            }

            throw new InvalidOperationException("ConnectionString not found in the configuration file.");
        }

        /// <summary>
        /// This method loads in the config file and reads it.
        /// </summary>
        /// <param name="filePath">The path of where the config file is on your system</param>
        /// <returns>Each line read in</returns>
        private Dictionary<string, string> LoadConfig(string filePath)
        {
            var config = new Dictionary<string, string>();

            try
            {
                Console.WriteLine($"Reading configuration from file: {filePath}");

                // Read the first line as the connection string
                var lines = File.ReadAllLines(filePath, Encoding.UTF8);
                if (lines.Length > 0)
                {
                    var connectionString = lines[0].Trim();
                    config["ConnectionString"] = connectionString;
                    Console.WriteLine($"Loaded: ConnectionString = {connectionString}");
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

