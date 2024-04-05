using Microsoft.Extensions.Configuration;
using Serilog;
using System.Reflection;

namespace PTO.Utilities
{
    public static class AppConfigUtil
    {
        private static readonly IConfiguration Configuration;
        private static readonly ILogger Logger; 
        private const string ConfigFile = "appsettings.json";

        static AppConfigUtil()
        {
            // Configure Serilog to write to the console
            Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            var directoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var projectLocation = directoryPath.Substring(0, directoryPath.Length - 16);
            
            Configuration = new ConfigurationBuilder()
                .SetBasePath(projectLocation)
                .AddJsonFile(ConfigFile) // Adjust file name and path if necessary
                .Build();
        }

        public static string? GetAppSetting(string key)
        {
            try
            {
                string? value = Configuration[key];

                if (value == null)
                {
                    Logger.Warning($"Key '{key}' not found in the configuration file '{ConfigFile}'.");
                }

                return value;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error reading configuration fiel '{ConfigFile}': {ex.Message}");
                return null;
            }
        }
    }
}
