using DevoceanExample.WebApi.WeatherForecast;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices.JavaScript;

namespace DevoceanExample.WebApi.Infrastructure.Persistence;

public static class DbContextSeedHelper
{
    public static async ValueTask<int> SeedData(ApplicationDbContext? context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        await SeedWeatherForecasts(context);
        return await context.SaveChangesAsync();
    }

    private static async Task SeedWeatherForecasts(ApplicationDbContext context)
    {
        if (!await context.WeatherForecast.AnyAsync())
        {
            var today = DateTime.Today;
            
            await context.WeatherForecast.AddRangeAsync(new List<WeatherForecastEntity>
            {
                new WeatherForecastEntity
                {
                    Summary = "Forecast 1",
                    Date = new DateOnly(today.Year, today.Month, today.Day),
                    TemperatureC = 30
                },
                new WeatherForecastEntity
                {
                    Summary = "Forecast 2",
                    Date = new DateOnly(today.Year, today.Month, today.Day + 1),
                    TemperatureC = 40
                },
                new WeatherForecastEntity
                {
                    Summary = "Forecast 3",
                    Date = new DateOnly(today.Year, today.Month, today.Day + 3),
                    TemperatureC = 24
                },
            });
        }
    }
}