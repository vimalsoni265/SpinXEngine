using log4net.Config;
using SpinXEngine.Core;
using System.Xml;

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
    var logDir = builder.Configuration["Logging:Log4Net:LogDirectory"] ?? "logs";
    var applicationName = builder.Configuration["Application"] ?? "SpinXEngine";
    logDir = Path.Combine(logDir, applicationName);

    // 2. Ensure the log directory exists with proper error handling
    try
    {
        if (!Directory.Exists(logDir))
        {
            Directory.CreateDirectory(logDir);
        }
    }
    catch (Exception ex)
    {
        // Fallback to a directory we know we can write to
        Console.WriteLine($"Failed to create log directory: {ex.Message}");
        logDir = Path.Combine(Path.GetTempPath(), "SpinXEngine", "logs");
        Directory.CreateDirectory(logDir);
    }

    // 3. Load and modify log4net.config before applying
    var log4NetConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "log4net.config");
    XmlDocument log4netConfig = new XmlDocument();
    log4netConfig.Load(File.OpenRead(log4NetConfigPath));

    // 4. Replace {LOG_DIR} placeholder in <file> element(s)
    var fileNodes = log4netConfig.SelectNodes("//file");
    foreach (XmlNode node in fileNodes!)
    {
        if (node.Attributes?["value"] != null)
        {
            node.Attributes["value"].Value = Path.Combine(logDir, "log-");
        }
    }

    // 5. Use in-memory stream for the updated config
    using (var ms = new MemoryStream() { Position = 0 })
    {
        log4netConfig.Save(ms);
        var logRepository = log4net.LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
        XmlConfigurator.Configure(logRepository, ms);
    }

    // 6. Register log4net as a provider (so ILogger<T> works everywhere)
    builder.Logging.ClearProviders();
    builder.Logging.AddLog4Net(log4NetConfigPath);
}
