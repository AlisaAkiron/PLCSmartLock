using Microsoft.EntityFrameworkCore;

namespace PLCHost.Database;

public class PlcHostDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public PlcHostDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public DbSet<Persistent> Persistent { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = "Data Source=" +
            $"{Path.Combine(_configuration.GetValue<string>("DataDirectory"), "data.db")}";
        optionsBuilder.UseSqlite(connectionString, builder =>
            builder.MigrationsAssembly("PLCHost"));
    }
}
