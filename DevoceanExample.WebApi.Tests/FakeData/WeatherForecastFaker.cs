using Bogus;
using DevoceanExample.WebApi.WeatherForecast.Apis.Models;
using DevoceanExample.WebApi.WeatherForecast.UseCases;

namespace DevoceanExample.WebApi.Tests.FakeData;

public static class WeatherForecastFaker
{
    public static Faker<WeatherForecastModel> GetFakerWeatherForecast()
        => new Faker<WeatherForecastModel>("pt_BR")
            .RuleFor(p => p.Date, (f, m) => f.Date.SoonDateOnly(10))
            .RuleFor(p => p.TemperatureC, (f, m) => f.Random.Int(20, 42))
            .RuleFor(p => p.Summary, (f, m) => f.Lorem.Sentences(3, " "));
    public static Faker<CreateWeatherForecast> GetFakerCreateWeatherForecast()
        => new Faker<CreateWeatherForecast>("pt_BR")
            .RuleFor(p => p.Date, (f, m) => f.Date.SoonDateOnly(10))
            .RuleFor(p => p.TemperatureC, (f, m) => f.Random.Int(20, 42))
            .RuleFor(p => p.Summary, (f, m) => f.Lorem.Sentences(3, " "));

    public static WeatherForecastModel GetWeatherForecast(string? ruleSets = null)
        => GetFakerWeatherForecast().Generate(ruleSets);

    public static List<WeatherForecastModel> GetWeatherForecast(int count, string? ruleSets = null)
        => GetFakerWeatherForecast().Generate(count, ruleSets);

    public static WeatherForecastModel GetEmpty()
        => new WeatherForecastModel();
}