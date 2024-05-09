using System;
using System.IO;
using System.Text.Json;

namespace SS.Backend.SharedNamespace
{
    public enum ScheduleUnit
    {
        Minutes,
        Days,
        Months
    }

    public class ArchivingServiceConfig
    {
        public DateTime StartDate { get; set; } = DateTime.Now;
        public int Interval { get; set; } = 1;
        public ScheduleUnit Unit { get; set; } = ScheduleUnit.Days;

        public static ArchivingServiceConfig LoadConfig(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string configJson = File.ReadAllText(filePath);
                    
                    JsonDocument doc = JsonDocument.Parse(configJson);
                    JsonElement root = doc.RootElement.GetProperty("Archiving");
                    DateTime startDate = root.GetProperty("StartDate").GetDateTime();


                    Console.WriteLine($"Start date: {startDate}");
                    Console.WriteLine($"Current date: {DateTime.Now}");
                    Console.WriteLine(startDate>DateTime.Now);

                    if (startDate < DateTime.Now)
                    {
                        Console.WriteLine("Start date is in the past, using current date.");
                        startDate = DateTime.Now;
                    }

                    int interval = root.GetProperty("Interval").GetInt32();

                    string unitString = root.GetProperty("Unit").GetString() ?? "Days";
                    ScheduleUnit unit;
                    if (!Enum.TryParse<ScheduleUnit>(unitString, true, out unit))
                    {
                        unit = ScheduleUnit.Days;
                    }

                    return new ArchivingServiceConfig
                    {
                        StartDate = startDate,
                        Interval = interval,
                        Unit = unit
                    };
                }
                else
                {
                    Console.WriteLine($"Configuration file not found: {filePath}, using defaults.");
                    return new ArchivingServiceConfig();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading configuration file: {ex.Message}, using defaults.");
                return new ArchivingServiceConfig();
            }
        }
    }
}
