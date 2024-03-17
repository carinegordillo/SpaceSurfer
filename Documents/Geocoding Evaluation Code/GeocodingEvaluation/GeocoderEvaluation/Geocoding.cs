using GoogleMapsApi;
using GoogleMapsApi.Entities.Geocoding.Request;
using System.Diagnostics;

namespace GeocodingEvaluation
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var googleApiKey = ReadGoogleApiKeyFromConfig();
            if (string.IsNullOrEmpty(googleApiKey))
            {
                Console.WriteLine("Google API key not found in config file.");
                return;
            }

            await EvaluateGoogleGeocodingAPI(googleApiKey);
            // await EvaluateOpenCageGeocodingAPI();
        }

        static async Task EvaluateGoogleGeocodingAPI(string apiKey)
        {
            var stopwatch = new Stopwatch();
            var request = new GeocodingRequest
            {
                Address = "314 South Oxford, Los Angeles, CA",
                ApiKey = apiKey
            };

            try
            {
                stopwatch.Start();
                var response = await GoogleMaps.Geocode.QueryAsync(request);
                var results = response.Results.ToList(); // Convert to list

                if (results.Any())
                {
                    var location = results[0].Geometry.Location;
                    stopwatch.Stop();
                    var responseTime = stopwatch.ElapsedMilliseconds;
                    Console.WriteLine($"Google Geocoding API Response Time: {responseTime} milliseconds");

                    var precisionPoints = GetPrecisionPoints(location.Latitude, location.Longitude);
                    Console.WriteLine($"Google Geocoding API Precision Points: {precisionPoints}");
                }
                else
                {
                    stopwatch.Stop();
                    var responseTime = stopwatch.ElapsedMilliseconds;
                    Console.WriteLine($"Google Geocoding API Response Time: {responseTime} milliseconds");
                    Console.WriteLine("No results found in Google Geocoding API response.");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while evaluating Google Geocoding API: {ex.Message}");
            }
        }



        //static async Task EvaluateOpenCageGeocodingAPI()
        //{
        //    var stopwatch = new Stopwatch();
        //    var geocoder = new Geocoder("YOUR_API_KEY");

        //    try
        //    {
        //        var address = "81 Hanover St, Edinburgh EH2 1EE, UK";
        //        stopwatch.Start();
        //        var result = await geocoder.GeocodeAsync(address);
        //        stopwatch.Stop();
        //        var responseTime = stopwatch.ElapsedMilliseconds;
        //        Console.WriteLine($"OpenCage Geocoding API Response Time: {responseTime} milliseconds");

        //        // var precisionPoints = GetPrecisionPoints(result.Geometry.Latitude, result.Geometry.Longitude);
        //        // Console.WriteLine($"OpenCage Geocoding API Precision Points: {precisionPoints}");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error occurred while evaluating OpenCage Geocoding API: {ex.Message}");
        //    }
        //}

        static int GetPrecisionPoints(double latitude, double longitude)
        {
            int precision = 0;
            string latStr = latitude.ToString();
            string lonStr = longitude.ToString();

            int latDecimalIndex = latStr.IndexOf('.');
            int lonDecimalIndex = lonStr.IndexOf('.');

            if (latDecimalIndex > 0)
                precision += latStr.Length - latDecimalIndex - 1;

            if (lonDecimalIndex > 0)
                precision += lonStr.Length - lonDecimalIndex - 1;

            return precision;
        }

        static string ReadGoogleApiKeyFromConfig()
        {
            try
            {
                var configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.local.txt");
                Console.WriteLine($"Attempting to read config file from: {configFilePath}");

                if (!File.Exists(configFilePath))
                {
                    Console.WriteLine("Config file not found.");
                    return null;
                }

                return File.ReadAllText(configFilePath).Trim();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading Google API key from config file: {ex.Message}");
                return null;
            }
        }
    }
}
