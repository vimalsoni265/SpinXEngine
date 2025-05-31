using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpinXEngine.Common.Helpers;
using SpinXEngine.Repository.Context;
using SpinXEngine.Repository.Implementation;
using SpinXEngine.Repository.Interfaces;
using SpinXEngine.Repository.Models;
using System.Diagnostics.CodeAnalysis;

namespace SpinXEngine.Repository
{
    /// <summary>
    /// Provides methods for configuring dependency injection services in an application.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "All the methods of this class are void and static methods")]
    public static class RepositoryServices
    {
        /// <summary>
        /// Function to Register DI services
        /// </summary>
        /// <param name="services">The Instance of <seealso cref="IServiceCollection"/></param>
        public static void ConfigureDependencies(IServiceCollection services)
        {
            // Configure MongoDB settings
            services.Configure<MongoDbSettings>(options =>
            {
                options.ConnectionString = ConfigurableSettings.MongoDbSettings.ConnectionString;
                options.DatabaseName = ConfigurableSettings.MongoDbSettings.DatabaseName;
            });
            // Register SpinXEngineDbContext as a singleton
            services.AddSingleton<SpinXEngineDbContext>();

            // Configure GameSetting with values from configuration
            services.Configure<GameSetting>(options =>
            {
                // Get the configuration section.
                if (ConfigurableSettings.GameSettings is IConfigurationSection section)
                {
                    // Try to get specific values, with defaults if not present
                    int reelRows = int.TryParse(section["ReelRows"], out int rows) ? rows : 3;
                    int reelCols = int.TryParse(section["ReelColumns"], out int cols) ? cols : 5;

                    options.ReelRows = reelRows;
                    options.ReelColumns = reelCols;
                }
            });

            // Configure Repositories.
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(IPlayerRepository), typeof(PlayerRepository));
        }
    }
}
