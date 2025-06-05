using log4net;
using log4net.Config;
using System.Reflection;

namespace SpinXEngine.Api
{
    public class Log4NetConfiguration
    {
        /// <summary>
        /// Configures Log4Net for the application
        /// </summary>
        /// <param name="builder">The WebApplicationBuilder instance</param>
        public static void Configure(WebApplicationBuilder builder)
        {
            try
            {
                // 1. Get log directory path from appsettings.json
                var logDir = builder.Configuration["Logging:Log4Net:LogDirectory"]
                    ?? @"\\SpinXEngine\\Logs";

                // Ensure the log directory is rooted.
                if (!Path.IsPathRooted(logDir))
                {
                    logDir = Path.Combine(@"C:\Users\Public\Documents", logDir);
                }

                // 2. Ensure the log directory exists
                EnsureLogDirectoryExists(logDir);

                // 3. Configure Log4Net without creating temporary files
                ConfigureWithoutTempFile(builder, logDir);

                // 4. Register Log4Net logger
                var startupLogger = LogManager.GetLogger(typeof(Program));
                startupLogger.Info("Application startup - Logging system initialized");
            }
            catch (Exception ex)
            {
                // Fallback to console logging if Log4Net configuration fails
                Console.Error.WriteLine($"Failed to configure Log4Net: {ex.Message}");
                ConfigureFallbackLogging(builder);
            }
        }

        /// <summary>
        /// Ensures the log directory exists
        /// </summary>
        /// <param name="logDir">Path to the log directory</param>
        private static void EnsureLogDirectoryExists(string logDir)
        {
            if (string.IsNullOrEmpty(logDir))
            {
                throw new ArgumentNullException(nameof(logDir), "Log directory path cannot be null or empty");
            }

            if (!Directory.Exists(logDir))
            {
                try
                {
                    Directory.CreateDirectory(logDir);
                }
                catch (Exception ex)
                {
                    throw new DirectoryNotFoundException($"Failed to create log directory at {logDir}: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Configures Log4Net without creating temporary files
        /// </summary>
        /// <param name="builder">The WebApplicationBuilder instance</param>
        /// <param name="logDir">Path to the log directory</param>
        private static void ConfigureWithoutTempFile(WebApplicationBuilder builder, string logDir)
        {
            // 1. Load log4net.config
            var log4NetConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "log4net.config");
            if (!File.Exists(log4NetConfigPath))
            {
                throw new FileNotFoundException("log4net.config not found", log4NetConfigPath);
            }

            // 2. Configure logging repository
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());

            // 3. Use log4net's built-in property overrides instead of file modification
            // This avoids creating temporary files
            GlobalContext.Properties["LogDir"] = logDir;

            // 4. Configure using the original file
            XmlConfigurator.Configure(logRepository, new FileInfo(log4NetConfigPath));

            // 5. Register log4net with .NET Core logging
            builder.Logging.ClearProviders();
            builder.Logging.AddLog4Net(log4NetConfigPath);
        }

        /// <summary>
        /// Configures fallback logging when Log4Net configuration fails
        /// </summary>
        /// <param name="builder">The WebApplicationBuilder instance</param>
        private static void ConfigureFallbackLogging(WebApplicationBuilder builder)
        {
            // Ensure at least console logging is available if Log4Net fails
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            var logger = builder.Services.BuildServiceProvider()
                .GetRequiredService<ILogger<Program>>();

            logger.LogWarning("Using fallback logging configuration due to Log4Net configuration failure");
        }
    }
}
