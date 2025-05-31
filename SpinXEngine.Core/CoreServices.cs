using Microsoft.Extensions.DependencyInjection;
using SpinXEngine.Core.GameDesigner;
using SpinXEngine.Core.Implementation;
using SpinXEngine.Core.Interface;
using SpinXEngine.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpinXEngine.Core
{
    /// <summary>
    /// Class to Configure Services
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "All the methods of this class are void and static methods")]
    public static class CoreServices
    {
        /// <summary>
        /// Function to Register DI services
        /// </summary>
        /// <param name="services">The Instance of <seealso cref="IServiceCollection"/></param>
        public static void ConfigureDependencies(IServiceCollection services)
        {
            // Register repository services
            RepositoryServices.ConfigureDependencies(services);

            // Register core services
            services.AddTransient<SpinGame>();
            services .AddScoped<IPlayerService, PlayerService>();
        }
    }
}
