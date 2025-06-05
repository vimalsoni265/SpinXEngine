using SpinXEngine.Api;
using SpinXEngine.Api.Extensions;
using SpinXEngine.Core;

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
Log4NetConfiguration.Configure(builder);

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

// TODO: This can be removed if not needed
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
