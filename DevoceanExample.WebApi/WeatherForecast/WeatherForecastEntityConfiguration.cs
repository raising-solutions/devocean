using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevoceanExample.WebApi.WeatherForecast;

public class WeatherForecastEntityConfiguration : IEntityTypeConfiguration<WeatherForecastEntity> 
{
    public void Configure(EntityTypeBuilder<WeatherForecastEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.ToTable("forecast");
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.TemperatureC).HasColumnName("temperature_c");
        builder.Property(x => x.Date).HasColumnName("date");
        builder.Property(x => x.Summary).HasColumnName("summary");
    }
}