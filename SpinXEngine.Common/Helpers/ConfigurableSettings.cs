using Microsoft.Extensions.Configuration;

namespace SpinXEngine.Common.Helpers
{
    /// <summary>
    /// Provides a centralized location for managing configurable application settings.
    /// </summary>
    /// <remarks>This class is designed to store and retrieve application-wide settings that can be configured
    /// at runtime or during initialization. It is intended to be used as a static utility for accessing  settings
    /// without requiring instance creation.</remarks>
    public static class ConfigurableSettings
    {
        private static readonly IConfigurationRoot m_configurationRoot;

        public static MongoDbSettings MongoDbSettings { get; private set; }

        public static object GameSettings { get; private set; }
        static ConfigurableSettings()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            m_configurationRoot = builder.Build();

            MongoDbSettings = new MongoDbSettings
            {
                ConnectionString = m_configurationRoot["ConnectionString:MongoDB"]!,
                DatabaseName = m_configurationRoot["ConnectionString:MongoDB"]!.Split('/').Last()
            };

            GameSettings = m_configurationRoot.GetSection("GameSettings");
        }
    }


    public class MongoDbSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
