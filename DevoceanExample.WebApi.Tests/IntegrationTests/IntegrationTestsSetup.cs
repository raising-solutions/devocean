using Devocean.Tests;
using DevoceanExample.WebApi.Infrastructure.Persistence;
using DevoceanExample.WebApi.WeatherForecast;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace DevoceanExample.WebApi.Tests.IntegrationTests;

public abstract class IntegrationTestsSetup : IntegrationTestBase<Program, ApplicationDbContext>
{
    protected IntegrationTestsSetup(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper
            , seedFunc: SeedFuncBuilder(testOutputHelper)
            , shouldMigrate: true)
    {
    }

    protected async Task<WeatherForecastEntity> FindRandomWeatherForecast()
    {
        var queryFindRandomWeatherForecast = DbContext.WeatherForecast.AsNoTracking();

        var notificationsCount = queryFindRandomWeatherForecast.Count();

        return await queryFindRandomWeatherForecast
            .OrderBy(r => r.Id)
            .Skip(new Random().Next(1, notificationsCount))
            .Take(1)
            .FirstAsync();
    }

    private static readonly Func<ITestOutputHelper, Action<ApplicationDbContext>> SeedFuncBuilder =
        testOutputHelper => context =>
        {
            var affectedRows = RunSeed(context).Result;
            if (affectedRows > 0)
                testOutputHelper.WriteLine($"Seed gerou {affectedRows} registros no banco");
        };

    private static async ValueTask<int> RunSeed(ApplicationDbContext dbContext)
    {
        var affectedRows = await DbContextSeedHelper.SeedData(dbContext!);
        return affectedRows;
    }
}