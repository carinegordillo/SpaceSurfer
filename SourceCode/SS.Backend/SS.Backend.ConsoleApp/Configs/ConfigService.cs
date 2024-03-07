// namespace SS.Backend.ConsoleApp.Configs
// {
//     public sealed class ConfigService
//     {
//         public AppSpecificConfig GetConfiguration()
//         {
//             //using (var fs = File.OpenRead("C: /Users/epik1/source/repos/SS.Backend/SS.Backend.ConsoleApp/Configs/config.local.txt"))
//             //{
//             //    using (var ms = new MemoryStream())
//             //    {
//             //        fs.CopyTo(ms);

//             //        var byte = ms.ReadByte();
//             //    }
//             //}

//             var configs = new AppSpecificConfig();
//             using (var fs = File.OpenText("C: /Users/epik1/source/repos/SS.Backend/SS.Backend.ConsoleApp/Configs/config.local.txt"))
//             {
//                 while (!fs.EndOfStream)
//                 {
//                     var config = fs.ReadLine();

//                     var variableName = config?.Split("=")[0];
//                     var variableValue = config?.Split("=")[1];

//                     //entry level
//                     switch (variableName)
//                     {
//                         case "ConnectionString":
//                             configs.ConnectionString = variableValue;
//                             break;
//                         default:
//                             break;
//                     }

//                     //entry level that has coding experience
//                     var configType = typeof(AppSpecificConfig);

//                     var allProperties = configType.GetProperties();

//                     foreach (var p in allProperties)
//                     {
//                         config[p.Name] = variableValue; // if config is a dictionary
//                     }

//                 }
//             }

//             return configs;
//         }
//     }

//     // next step is to do whatever you want with the config after reading it in, below is another class (have it in another file)
//     public class AppSpecificConfig
//     {
//         public string ConnectionString { get; set; }
//         public int MaxRetryAttempts { get; set; }
//         public int TimeoutLimitInSeconds { get; set; }
//         public int MaxUploadCount { get; set; }
//         public string AllowedUploadFileTypes { get; set; }
//     }
// }
