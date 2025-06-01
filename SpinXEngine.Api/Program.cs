using log4net;
using log4net.Config;
using SpinXEngine.Api.Extensions;
using SpinXEngine.Core;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.ReportApiVersions = true;
});

// Configure log4net for logging
ConfigureLog4Net(builder);

// Configure Core Services DI
ConfigureServices(builder.Services);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseGlobalExceptionHandler();
app.UseAuthorization();
app.MapControllers();
app.Run();



/// <summary>
/// Function to Configure DI for Services
/// </summary>
/// <param name="services"></param>
static void ConfigureServices(IServiceCollection services)
{
    CoreServices.ConfigureDependencies(services);
}

/// <summary>
/// Function to Configure log4net for logging
/// </summary>
static void ConfigureLog4Net(WebApplicationBuilder builder)
{
    // 1. Get log directory path from appsettings.json (with fallback)
    var logDir = builder.Configuration["Logging:Log4Net:LogDirectory"];

    // 2. Ensure the log directory exists.
    if (!Directory.Exists(logDir))
    {
        Directory.CreateDirectory(logDir);
    }

    // 3. Load and modify log4net.config
    var log4NetConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "log4net.config");
    if (!File.Exists(log4NetConfigPath))
    {
        throw new FileNotFoundException("log4net.config not found", log4NetConfigPath);
    }

    // 4. Create a modified copy of the config
    var configContent = File.ReadAllText(log4NetConfigPath);
    configContent = configContent.Replace("{LOG_DIR}", logDir);

    var tempConfigPath = Path.Combine(Path.GetTempPath(), "log4net_modified.config");
    File.WriteAllText(tempConfigPath, configContent);

    // 5. Configure the logging system
    var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
    XmlConfigurator.Configure(logRepository, new FileInfo(tempConfigPath));

    // 6. Register log4net with .NET Core logging
    builder.Logging.ClearProviders();
    builder.Logging.AddLog4Net(tempConfigPath);

    // Add this after logging configuration
    var startupLogger = LogManager.GetLogger(typeof(Program));
    startupLogger.Info("Application startup - Logging system initialized");
}