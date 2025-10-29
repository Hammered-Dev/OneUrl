using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using OneUrlApi.Models;

class DatabaseService : DbContext
{
    public DbSet<UrlRecord> UrlRecords { get; set; }
    public DbSet<SettingsModel> Settings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        Env.Load();
        var host = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost";
        var port = Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432";
        var database = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "postgres";
        var user = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres";
        var password = Environment.GetEnvironmentVariable("POSTGRES_PASS");
        optionsBuilder.UseNpgsql($"Host={host}:{port};Username={user};Password={password};Database={database}");
        base.OnConfiguring(optionsBuilder);
    }
}