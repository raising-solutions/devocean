using Devocean.Core.Domain.common;

namespace DevoceanExample.WebApi.WeatherForecast;

public class WeatherForecastEntity : AuditableEntityBase<Guid>
{
    private int? _temperatureC;
    private int? _temperatureF;
    public DateOnly Date { get; set; }

    public int TemperatureC
    {
        get => _temperatureC ?? (_temperatureF is null ? 0 : (int)((_temperatureF - 32) * 5 / 9));
        set => _temperatureC = value;
    }

    public int TemperatureF
    {
        get => _temperatureF ?? (_temperatureF is null ? 0 : 32 + (int)(TemperatureC / 0.5556));
        set => _temperatureF = value;
    }

    public string? Summary { get; set; }

    public WeatherForecastEntity()
    {
    }

    public WeatherForecastEntity(DateOnly date, int temperatureC, string? summary)
    {
        Date = date;
        TemperatureC = temperatureC;
        Summary = summary;
    }
}