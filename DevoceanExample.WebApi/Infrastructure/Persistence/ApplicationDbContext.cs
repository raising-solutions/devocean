using Devocean.Core.Application.Interfaces;
using Devocean.Core.Infrastructure.Persistence.Data;
using DevoceanExample.WebApi.WeatherForecast;
using Microsoft.EntityFrameworkCore;

namespace DevoceanExample.WebApi.Infrastructure.Persistence;

public class ApplicationDbContext : DbContextBase<ApplicationDbContext>
{
    public static string Schema => "weatherForecast";

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
        IDateTime dateTime) : base(options, schema: Schema)
    {
    }

    public DbSet<WeatherForecastEntity> WeatherForecast { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        base.OnModelCreating(modelBuilder);
    }
}