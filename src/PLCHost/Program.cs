using System.Reflection;
using Microsoft.EntityFrameworkCore;
using PLCHost;
using Serilog;

#region Path

var assemblyPath = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory!.FullName;
var dataDirectory = Path.Combine(assemblyPath, "data");

if (Directory.Exists(dataDirectory) is false)
{
    Directory.CreateDirectory(dataDirectory);
}

#endregion

#region Configuration

var configurationBuilder = new ConfigurationBuilder();

configurationBuilder.AddJsonFile(Path.Combine(assemblyPath, "appsettings.json"));
if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
{
    configurationBuilder.AddJsonFile(Path.Combine(assemblyPath, "appsettings.Development.json"), true);
}

configurationBuilder.AddInMemoryCollection(new List<KeyValuePair<string, string>>
{
    new("AssemblyPath", assemblyPath),
    new ("DataDirectory", dataDirectory)
});

configurationBuilder.AddEnvironmentVariables("PLC:");
configurationBuilder.AddCommandLine(args);

var configuration = configurationBuilder.Build();

#endregion

#region Logger

var loggerBuilder = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
        path: Path.Combine(assemblyPath, "log/log-.log"),
        rollingInterval: RollingInterval.Day,
        shared: true)
    .Enrich.FromLogContext()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", Serilog.Events.LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Information();

if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
{
    loggerBuilder.MinimumLevel.Debug();
}

Log.Logger = loggerBuilder.CreateLogger();
Log.Logger.Information("Application start up...");

#endregion

#region Web App Builder

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Configuration.AddConfiguration(configuration);

builder.Services.AddHostedService<HostedService>();
builder.Services.AddDbContext<PlcHostDbContext>();
builder.Services.AddPlcHostServices();
builder.Services.AddPlcHostJobs();
builder.Services.AddMasaBlazor();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var app = builder.Build();

#endregion

#region Database

var dbFile = Path.Combine(configuration.GetValue<string>("DataDirectory"), "data.db");
if (File.Exists(dbFile) is false)
{
    using var scope = app.Services.CreateScope();
    await using var db = scope.ServiceProvider.GetService<PlcHostDbContext>();

    if (db is null)
    {
        Log.Logger.Fatal("DbContext instance is null");
        return;
    }

    await db.Database.MigrateAsync();
    await db.DisposeAsync();
}

#endregion

#region Pipeline

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

await app.RunAsync();

#endregion
