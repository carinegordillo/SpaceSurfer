using SS.Backend.SharedNamespace;
using System.Data;
using System.Data.SqlClient;

namespace SS.Backend.DataAccess
{
    public sealed class SealedPepperDAO
    {
        private readonly string _filePath;

        public SealedPepperDAO(string filePath)
        {
            _filePath = filePath;
        }

        public async Task<string> ReadPepperAsync()
        {
            try
            {
                using (StreamReader reader = new StreamReader(_filePath))
                {
                    return await reader.ReadToEndAsync();
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., file not found, no access permissions)
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }
    }
}