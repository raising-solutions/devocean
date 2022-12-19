namespace DevoceanExample.WebApi.WeatherForecast.Apis.Models;

public class WeatherForecastModel
{
    public DateOnly Date { get; set; }
    public int TemperatureC { get; set; }
    public int TemperatureF { get; set; }
    public string? Summary { get; set; }
}